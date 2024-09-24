using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using FaccToolkit.Persistence.EntityFramework.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FaccToolkit.Persistence.EntityFramework.AnemicDomain
{
    public class EntityRepository<TEntity, TId, TDbContext> : BaseRepository<TEntity, TId, TDbContext>, IEntityRepository<TEntity, TId>
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

        Task IEntityRepository<TEntity, TId>.UpdateAsync(TEntity entity, CancellationToken cancellationToken)
            => base.UpdateAsync(entity, cancellationToken);

        Task IEntityRepository<TEntity, TId>.DeleteAsync(TId id, CancellationToken cancellationToken)
            => base.DeleteAsync(id, cancellationToken);

        protected override TId GetId(TEntity model)
            => model.Id;
    }
}
