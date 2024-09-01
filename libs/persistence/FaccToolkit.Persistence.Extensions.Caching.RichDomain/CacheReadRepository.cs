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
        protected readonly ILogger<CacheReadRepository<TReadRepository, TAggregateRoot, TId>> _logger;

        public CacheReadRepository(ICacheFacade cacheFacade, TReadRepository dbReadRepository, ILogger<CacheReadRepository<TReadRepository, TAggregateRoot, TId>> logger)
        {
            _cacheFacade = cacheFacade;
            _dbReadRepository = dbReadRepository;
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
            
            aggregate = await _dbReadRepository.FindByIdAsync(id, cancellationToken);

            if (aggregate != null)
            {
                _logger.LogInformation("Setting {Key} on cache", key);
             
                await _cacheFacade.SetAsync(key, aggregate, cancellationToken);
            }

            return aggregate;
        }
    }
}
