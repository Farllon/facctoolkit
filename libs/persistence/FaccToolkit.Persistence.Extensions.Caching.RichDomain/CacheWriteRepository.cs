using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Domain.Rich;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.Extensions.Caching.RichDomain
{
    public class CacheWriteRepository<TWriteRepository, TAggregateRoot, TId> : IWriteRepository<TAggregateRoot, TId>
        where TWriteRepository : IWriteRepository<TAggregateRoot, TId>
        where TAggregateRoot : class, IAggregateRoot<TId>
        where TId : IEquatable<TId>
    {
        protected readonly ICacheFacade _cacheFacade;
        protected readonly TWriteRepository _dbWriteRepository;

        public CacheWriteRepository(ICacheFacade cacheFacade, TWriteRepository dbReadRepository, ILogger<CacheWriteRepository<TWriteRepository, TAggregateRoot, TId>> logger)
        {
            _cacheFacade = cacheFacade;
            _dbWriteRepository = dbReadRepository;
        }

        public virtual async Task InsertAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            await _dbWriteRepository.InsertAsync(aggregate, cancellationToken);

            var key = _cacheFacade.GenerateKey<TAggregateRoot>(aggregate.Id.ToString());

            await _cacheFacade.SetAsync(key, aggregate, cancellationToken);
        }

        public virtual async Task InsertAsync(IEnumerable<TAggregateRoot> aggregate, CancellationToken cancellationToken)
        {
            await _dbWriteRepository.InsertAsync(aggregate, cancellationToken);

            var tasks = aggregate.Select(entity =>
            {
                var key = _cacheFacade.GenerateKey<TAggregateRoot>(entity.Id.ToString());

                return _cacheFacade.SetAsync(key, entity, cancellationToken);
            });

            await Task.WhenAll(tasks);
        }

        public virtual async Task UpdateAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            await _dbWriteRepository.UpdateAsync(aggregate, cancellationToken);

            var key = _cacheFacade.GenerateKey<TAggregateRoot>(aggregate.Id.ToString());

            await _cacheFacade.SetAsync(key, aggregate, cancellationToken);
        }

        public virtual async Task DeleteAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            await _dbWriteRepository.DeleteAsync(aggregate, cancellationToken);

            var key = _cacheFacade.GenerateKey<TAggregateRoot>(aggregate.ToString());

            await _cacheFacade.ExpiryAsync(key, cancellationToken);
        }
    }
}
