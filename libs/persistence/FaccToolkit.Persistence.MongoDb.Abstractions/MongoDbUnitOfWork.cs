using FaccToolkit.Persistence.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.Abstractions
{
    public sealed class MongoDbUnitOfWork<TDbContext> : UnitOfWork
        where TDbContext : MongoDbContext
    {
        private readonly TDbContext _context;

        public MongoDbUnitOfWork(IServiceProvider provider, TDbContext context) : base(provider) 
        { 
            _context = context;
        }

        public override Task<bool> IsInTransactionAsync(CancellationToken cancellationToken)
            => Task.FromResult(_context.CurrentSession.IsInTransaction);

        public override Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            _context.StartTransaction();

            return Task.CompletedTask;
        }

        public override Task CommitTransactionAsync(CancellationToken cancellationToken)
            => _context.CommitTransactionAsync(cancellationToken);

        public override Task AbortTransactionAsync(CancellationToken cancellationToken)
            => _context.AbortTransactionAsync(cancellationToken);

        public override void Dispose()
        {
            _context.Dispose();
        }
    }
}
