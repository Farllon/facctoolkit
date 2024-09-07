using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using FaccToolkit.Persistence.EntityFramework.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FaccToolkit.Persistence.EntityFramework.AnemicDomain
{
    public class EntityRepository<TEntity, TId, TDbContext> : BaseRepository<TEntity, TDbContext>, IEntityRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
        where TDbContext : DbContext
    {
        protected readonly DbSet<TEntity> _entities;

        public EntityRepository(TDbContext context, ILogger<EntityRepository<TEntity, TId, TDbContext>> logger)
            : base(context, logger)
        {
            _entities = _context.Set<TEntity>();
        }

        public async Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding {Model} model with {Id} id in database", typeof(TEntity), id);

            try
            {
                var entity = await _entities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken)
                    .ConfigureAwait(false);

                _logger.LogInformation("Successfully found {Model} model with {Id} id", typeof(TEntity), id);

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to found {Model} model with {Id} id", typeof(TEntity), id);
                
                throw;
            }
        }

        public Task InsertAsync(TEntity entity, CancellationToken cancellationToken)
            => InsertAsync(e => e.Id, entity, cancellationToken);

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
            => UpdateAsync(e => e.Id, entity, cancellationToken);

        public Task DeleteAsync(TId id, CancellationToken cancellationToken)
            => DeleteAsync<TId>(id, cancellationToken);
    }
}
