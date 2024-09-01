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
    public class CacheWriteRepository<TWriteRepository, TEntity, TId> : IWriteRepository<TEntity, TId>
        where TWriteRepository : IWriteRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        protected readonly ICacheFacade _cacheFacade;
        protected readonly TWriteRepository _dbWriteRepository;
        protected readonly ILogger<CacheWriteRepository<TWriteRepository, TEntity, TId>> _logger;

        public CacheWriteRepository(ICacheFacade cacheFacade, TWriteRepository dbReadRepository, ILogger<CacheWriteRepository<TWriteRepository, TEntity, TId>> logger)
        {
            _cacheFacade = cacheFacade;
            _dbWriteRepository = dbReadRepository;
            _logger = logger;
        }

        public virtual async Task InsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Insert cache flow");

            await _dbWriteRepository.InsertAsync(entity, cancellationToken);

            _logger.LogInformation("Generating key");

            var key = _cacheFacade.GenerateKey<TEntity>(entity.Id.ToString());

            _logger.LogInformation("Setting {Key} on cache", key);

            await _cacheFacade.SetAsync(key, entity, cancellationToken);
        }

        public virtual async Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Insert cache flow");

            await _dbWriteRepository.InsertAsync(entities, cancellationToken);

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

            await _dbWriteRepository.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("Generating key");

            var key = _cacheFacade.GenerateKey<TEntity>(entity.Id.ToString());

            _logger.LogInformation("Setting {Key} on cache", key);

            await _cacheFacade.SetAsync(key, entity, cancellationToken);
        }

        public virtual async Task DeleteAsync(TId id, CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Delete cache flow");

            await _dbWriteRepository.DeleteAsync(id, cancellationToken);
            
            _logger.LogInformation("Generating key");

            var key = _cacheFacade.GenerateKey<TEntity>(id.ToString());

            _logger.LogInformation("Exipiring {Key} from cache", key);
            
            await _cacheFacade.ExpiryAsync(key, cancellationToken);
        }
    }
}
