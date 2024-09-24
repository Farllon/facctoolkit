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
    public class CacheRepository<TRepository, TAggregateRoot, TId> : IAggregateRepository<TAggregateRoot, TId>
        where TRepository : IAggregateRepository<TAggregateRoot, TId>
        where TAggregateRoot : class, IAggregateRoot<TId>
        where TId : IEquatable<TId>
    {
        protected readonly ICacheFacade _cacheFacade;
        protected readonly TRepository _dbRepository;
        protected readonly ILogger<CacheRepository<TRepository, TAggregateRoot, TId>> _logger;

        public CacheRepository(ICacheFacade cacheFacade, TRepository dbReadRepository, ILogger<CacheRepository<TRepository, TAggregateRoot, TId>> logger)
        {
            _cacheFacade = cacheFacade;
            _dbRepository = dbReadRepository;
            _logger = logger;
        }

        public virtual async Task<TAggregateRoot?> FindByIdAsync(TId id, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Find in cache by id flow");

            _logger.LogInformation("Generating key");

            var key = _cacheFacade.GenerateKey<TAggregateRoot>(id.ToString());

            _logger.LogInformation("Trying to get {Key} from cache", key);

            var aggregate = await _cacheFacade.TryGetAsync<TAggregateRoot>(key, cancellationToken);

            if (aggregate != null)
                return aggregate;

            _logger.LogInformation("Not found on cache. Trying to get from database");

            aggregate = await _dbRepository.FindByIdAsync(id, cancellationToken);

            if (aggregate != null)
            {
                _logger.LogInformation("Setting {Key} on cache", key);

                await _cacheFacade.SetAsync(key, aggregate, cancellationToken);
            }

            return aggregate;
        }

        public virtual async Task InsertAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Insert cache flow");
         
            await _dbRepository.InsertAsync(aggregate, cancellationToken);

            _logger.LogInformation("Generating key");
            
            var key = _cacheFacade.GenerateKey<TAggregateRoot>(aggregate.Id.ToString());

            _logger.LogInformation("Setting {Key} on cache", key);
            
            await _cacheFacade.SetAsync(key, aggregate, cancellationToken);
        }

        public virtual async Task InsertAsync(IEnumerable<TAggregateRoot> aggregate, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Insert cache flow");
            
            await _dbRepository.InsertAsync(aggregate, cancellationToken);

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
            
            await _dbRepository.UpdateAsync(aggregate, cancellationToken);

            _logger.LogInformation("Generating key");

            var key = _cacheFacade.GenerateKey<TAggregateRoot>(aggregate.Id.ToString());

            _logger.LogInformation("Expiring {Key} from cache", key);

            await _cacheFacade.ExpiryAsync(key, cancellationToken);
        }

        public virtual async Task DeleteAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Delete cache flow");
            
            await _dbRepository.DeleteAsync(aggregate, cancellationToken);

            _logger.LogInformation("Generating key");
            
            var key = _cacheFacade.GenerateKey<TAggregateRoot>(aggregate.Id.ToString());

            _logger.LogInformation("Expiring {Key} from cache", key);
            
            await _cacheFacade.ExpiryAsync(key, cancellationToken);
        }
    }
}
