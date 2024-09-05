using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.EntityFramework.AnemicDomain
{
    public class WriteRepository<TEntity, TId, TDbContext> : IWriteRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
        where TDbContext : DbContext
    {
        protected readonly TDbContext _context;
        protected readonly DbSet<TEntity> _entities;

        public WriteRepository(TDbContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        public Task InsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
        }

        public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
