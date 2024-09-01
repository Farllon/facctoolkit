using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.Extensions.Caching.AnemicDomain
{
    public class CacheReadRepository<TReadRepository, TEntity, TId> : IReadRepository<TEntity, TId>
        where TReadRepository : IReadRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        protected readonly ICacheFacade _cacheFacade;
        protected readonly TReadRepository _dbReadRepository;
        protected readonly ILogger<CacheReadRepository<TReadRepository, TEntity, TId>> _logger;

        public CacheReadRepository(ICacheFacade cacheFacade, TReadRepository dbReadRepository, ILogger<CacheReadRepository<TReadRepository, TEntity, TId>> logger)
        {
            _cacheFacade = cacheFacade;
            _dbReadRepository = dbReadRepository;
            _logger = logger;
        }

        public virtual async Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Find in cache by id flow");

            _logger.LogInformation("Generating key");

            var key = _cacheFacade.GenerateKey<TEntity>(id.ToString());

            _logger.LogInformation("Trying to get {Key} from cache", key);

            var entity = await _cacheFacade.TryGetAsync<TEntity>(key, cancellationToken);

            if (entity != null)
                return entity;

            _logger.LogInformation("Not found on cache. Trying to get from database");

            entity = await _dbReadRepository.FindByIdAsync(id, cancellationToken);

            if (entity != null)
            {
                _logger.LogInformation("Setting {Key} on cache", key);

                await _cacheFacade.SetAsync(key, entity, cancellationToken);
            }

            return entity;
        }
    }
}
