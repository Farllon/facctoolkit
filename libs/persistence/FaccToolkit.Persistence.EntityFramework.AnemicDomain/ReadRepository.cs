using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using Microsoft.EntityFrameworkCore;

namespace FaccToolkit.Persistence.EntityFramework.AnemicDomain
{
    public class ReadRepository<TEntity, TId, TDbContext> : IReadRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
        where TDbContext : DbContext
    {
        protected readonly TDbContext _context;
        protected readonly DbSet<TEntity> _entities;

        public ReadRepository(TDbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken)
            => _entities.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id.Equals(id), cancellationToken);
    }
}
