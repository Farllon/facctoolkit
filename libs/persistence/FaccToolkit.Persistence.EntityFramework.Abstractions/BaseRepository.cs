using FaccToolkit.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FaccToolkit.Persistence.EntityFramework.Abstractions
{
    public abstract class BaseRepository<TModel, TId, TDbContext> : ModelRepository<TModel, TId>, IAsyncDisposable
        where TModel : class
        where TDbContext : DbContext
        where TId : IEquatable<TId>
    {
        protected readonly TDbContext _context;

        public BaseRepository(TDbContext context, ILogger<BaseRepository<TModel, TId, TDbContext>> logger)
            : base(logger)
        {
            _context = context;
        }

        public async ValueTask DisposeAsync()
        {
            _logger.LogInformation("Saving changes to the database and disposing {Context}", typeof(TDbContext));

            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);

                _logger.LogInformation("Successfully saved changes and disposed {Context}", typeof(TDbContext));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save changes in {Context}", typeof(TDbContext));

                throw;
            }

            GC.SuppressFinalize(this);
        }

        protected override Task<TModel?> InternalFindByIdAsync(TId id, CancellationToken cancellationToken)
            => _context
                .Set<TModel>()
                .FindAsync(new object?[] { id }, cancellationToken: cancellationToken)
                .AsTask();

        protected override Task InternalInsertAsync(TModel model, CancellationToken cancellationToken)
            => _context
                .Set<TModel>()
                .AddAsync(model, cancellationToken)
                .AsTask();

        protected override Task InternalInsertAsync(IEnumerable<TModel> models, CancellationToken cancellationToken)
            => _context
                .Set<TModel>()
                .AddRangeAsync(models, cancellationToken);

        protected override async Task<TModel?> InternalUpdateAsync(TModel model, CancellationToken cancellationToken)
        {
            var id = GetId(model);

            var found = await _context
                .Set<TModel>()
                .FindAsync(new object?[] { id }, cancellationToken: cancellationToken);

            if (found is null)
                return null;

            _context
                .Set<TModel>()
                .Update(model);

            return found;
        }

        protected override async Task<TModel?> InternalDeleteAsync(TId id, CancellationToken cancellationToken)
        {
            var found = await _context
                .Set<TModel>()
                .FindAsync(new object?[] { id }, cancellationToken: cancellationToken);

            if (found is null)
                return null;

            _context
                .Set<TModel>()
                .Remove(found);

            return found;
        }
    }
}
