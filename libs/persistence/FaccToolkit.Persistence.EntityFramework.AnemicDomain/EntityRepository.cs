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

        public Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken)
            => base.FindByIdAsync(e => e.Id, id, cancellationToken);

        public Task InsertAsync(TEntity entity, CancellationToken cancellationToken)
            => base.InsertAsync(e => e.Id, entity, cancellationToken);

        public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
            => base.InsertAsync<TId>(entities, cancellationToken);

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
            => base.UpdateAsync(e => e.Id, entity, cancellationToken);

        public Task DeleteAsync(TId id, CancellationToken cancellationToken)
            => base.DeleteAsync(e => e.Id, id, cancellationToken);
    }
}
