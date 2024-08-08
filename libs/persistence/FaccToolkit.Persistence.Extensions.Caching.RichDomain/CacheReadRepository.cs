using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Domain.Rich;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.Extensions.Caching.RichDomain
{
    public class CacheReadRepository<TReadRepository, TAggregateRoot, TId> : IReadRepository<TAggregateRoot, TId>
        where TReadRepository : IReadRepository<TAggregateRoot, TId>
        where TAggregateRoot : class, IAggregateRoot<TId>
        where TId : IEquatable<TId>
    {
        protected readonly ICacheFacade _cacheFacade;
        protected readonly TReadRepository _dbReadRepository;

        public CacheReadRepository(ICacheFacade cacheFacade, TReadRepository dbReadRepository, ILogger<CacheReadRepository<TReadRepository, TAggregateRoot, TId>> logger)
        {
            _cacheFacade = cacheFacade;
            _dbReadRepository = dbReadRepository;
        }

        public virtual async Task<TAggregateRoot?> FindByIdAsync(TId id, CancellationToken cancellationToken)
        {
            var key = _cacheFacade.GenerateKey<TAggregateRoot>(id.ToString());
            var aggregate = await _cacheFacade.TryGetAsync<TAggregateRoot>(key, cancellationToken);

            if (aggregate != null)
                return aggregate;

            aggregate = await _dbReadRepository.FindByIdAsync(id, cancellationToken);

            if (aggregate != null)
                await _cacheFacade.SetAsync(key, aggregate, cancellationToken);

            return aggregate;
        }
    }
}
