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

        public CacheReadRepository(ICacheFacade cacheFacade, TReadRepository dbReadRepository, ILogger<CacheReadRepository<TReadRepository, TEntity, TId>> logger)
        {
            _cacheFacade = cacheFacade;
            _dbReadRepository = dbReadRepository;
        }

        public virtual async Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken)
        {
            var key = _cacheFacade.GenerateKey<TEntity>(id.ToString());
            var entity = await _cacheFacade.TryGetAsync<TEntity>(key, cancellationToken);

            if (entity != null)
                return entity;

            entity = await _dbReadRepository.FindByIdAsync(id, cancellationToken);

            if (entity != null)
                await _cacheFacade.SetAsync(key, entity, cancellationToken);

            return entity;
        }
    }
}
