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
        protected readonly ILogger<CacheWriteRepository<TWriteRepository, TAggregateRoot, TId>> _logger;

        public CacheWriteRepository(ICacheFacade cacheFacade, TWriteRepository dbReadRepository, ILogger<CacheWriteRepository<TWriteRepository, TAggregateRoot, TId>> logger)
        {
            _cacheFacade = cacheFacade;
            _dbWriteRepository = dbReadRepository;
            _logger = logger;
        }

        public virtual async Task InsertAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Insert cache flow");
         
            await _dbWriteRepository.InsertAsync(aggregate, cancellationToken);

            _logger.LogInformation("Generating key");
            
            var key = _cacheFacade.GenerateKey<TAggregateRoot>(aggregate.Id.ToString());

            _logger.LogInformation("Setting {Key} on cache", key);
            
            await _cacheFacade.SetAsync(key, aggregate, cancellationToken);
        }

        public virtual async Task InsertAsync(IEnumerable<TAggregateRoot> aggregate, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Insert cache flow");
            
            await _dbWriteRepository.InsertAsync(aggregate, cancellationToken);

            var tasks = aggregate.Select(entity =>
            {
                _logger.LogInformation("Generating key");
                    
                var key = _cacheFacade.GenerateKey<TAggregateRoot>(entity.Id.ToString());

                _logger.LogInformation("Setting {Key} on cache", key);
                
                return _cacheFacade.SetAsync(key, entity, cancellationToken);
            });

            await Task.WhenAll(tasks);
        }

        public virtual async Task UpdateAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Update cache flow");
            
            await _dbWriteRepository.UpdateAsync(aggregate, cancellationToken);

            _logger.LogInformation("Generating key");

            var key = _cacheFacade.GenerateKey<TAggregateRoot>(aggregate.Id.ToString());

            _logger.LogInformation("Setting {Key} on cache", key);
            
            await _cacheFacade.SetAsync(key, aggregate, cancellationToken);
        }

        public virtual async Task DeleteAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Delete cache flow");
            
            await _dbWriteRepository.DeleteAsync(aggregate, cancellationToken);

            _logger.LogInformation("Generating key");
            
            var key = _cacheFacade.GenerateKey<TAggregateRoot>(aggregate.ToString());

            _logger.LogInformation("Expiring {Key} from cache", key);
            
            await _cacheFacade.ExpiryAsync(key, cancellationToken);
        }
    }
}
