using FaccToolkit.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FaccToolkit.Persistence.EntityFramework.Abstractions
{
    public sealed class EfUnitOfWork<TDbContext> : UnitOfWork
        where TDbContext : DbContext
    {
        private readonly TDbContext _context;

        public EfUnitOfWork(IServiceProvider provider, TDbContext context) : base(provider)
        {
            _context = context;
        }

        public override Task AbortTransactionAsync(CancellationToken cancellationToken)
            => _context.Database.RollbackTransactionAsync(cancellationToken);

        public override Task BeginTransactionAsync(CancellationToken cancellationToken)
            => _context.Database.BeginTransactionAsync(cancellationToken);

        public override Task CommitTransactionAsync(CancellationToken cancellationToken)
            => _context.Database.CommitTransactionAsync(cancellationToken);

        public override Task<bool> IsInTransactionAsync(CancellationToken cancellationToken)
            => Task.FromResult(_context.Database.CurrentTransaction is not null);

        public override void Dispose()
        {
            _context.Dispose();
        }
    }
}
