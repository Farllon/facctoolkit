using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.Extensions.Caching.AnemicDomain
{
    public class CacheRepository<TRepository, TEntity, TId> : IEntityRepository<TEntity, TId>
        where TRepository : IEntityRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        protected readonly ICacheFacade _cacheFacade;
        protected readonly TRepository _dbRepository;
        protected readonly ILogger<CacheRepository<TRepository, TEntity, TId>> _logger;

        public CacheRepository(ICacheFacade cacheFacade, TRepository dbReadRepository, ILogger<CacheRepository<TRepository, TEntity, TId>> logger)
        {
            _cacheFacade = cacheFacade;
            _dbRepository = dbReadRepository;
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

            entity = await _dbRepository.FindByIdAsync(id, cancellationToken);

            if (entity != null)
            {
                _logger.LogInformation("Setting {Key} on cache", key);

                await _cacheFacade.SetAsync(key, entity, cancellationToken);
            }

            return entity;
        }

        public virtual async Task InsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Insert cache flow");

            await _dbRepository.InsertAsync(entity, cancellationToken);

            _logger.LogInformation("Generating key");

            var key = _cacheFacade.GenerateKey<TEntity>(entity.Id.ToString());

            _logger.LogInformation("Setting {Key} on cache", key);

            await _cacheFacade.SetAsync(key, entity, cancellationToken);
        }

        public virtual async Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Insert cache flow");

            await _dbRepository.InsertAsync(entities, cancellationToken);

            var tasks = entities.Select(entity =>
            {
                _logger.LogInformation("Generating key");

                var key = _cacheFacade.GenerateKey<TEntity>(entity.Id.ToString());

                _logger.LogInformation("Setting {Key} on cache", key);

                return _cacheFacade.SetAsync(key, entity, cancellationToken);
            });

            await Task.WhenAll(tasks);
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Update cache flow");

            await _dbRepository.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("Generating key");

            var key = _cacheFacade.GenerateKey<TEntity>(entity.Id.ToString());

            _logger.LogInformation("Setting {Key} on cache", key);

            await _cacheFacade.SetAsync(key, entity, cancellationToken);
        }

        public virtual async Task DeleteAsync(TId id, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Delete cache flow");

            await _dbRepository.DeleteAsync(id, cancellationToken);
            
            _logger.LogInformation("Generating key");

            var key = _cacheFacade.GenerateKey<TEntity>(id.ToString());

            _logger.LogInformation("Exipiring {Key} from cache", key);
            
            await _cacheFacade.ExpiryAsync(key, cancellationToken);
        }
    }
}
