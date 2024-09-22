using FaccToolkit.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FaccToolkit.Persistence.EntityFramework.Abstractions
{
    public class BaseRepository<TModel, TDbContext> : ModelRepository<TModel>, IAsyncDisposable
        where TModel : class
        where TDbContext : DbContext
    {
        protected readonly TDbContext _context;

        public BaseRepository(TDbContext context, ILogger<BaseRepository<TModel, TDbContext>> logger)
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

        protected override Task<TModel?> InternalFindByIdAsync<TId>(Func<TModel, TId> idSelector, TId id, CancellationToken cancellationToken)
            => _context
                .Set<TModel>()
                .AsNoTracking()
                .FirstOrDefaultAsync(GetFilter(model => idSelector(model)!.Equals(id)), cancellationToken);

        protected override Task InternalInsertAsync(TModel model, CancellationToken cancellationToken)
            => _context
                .Set<TModel>()
                .AddAsync(model, cancellationToken)
                .AsTask();

        protected override Task InternalInsertAsync(IEnumerable<TModel> models, CancellationToken cancellationToken)
            => _context
                .Set<TModel>()
                .AddRangeAsync(models, cancellationToken);

        protected override async Task<TModel?> InternalUpdateAsync<TId>(Func<TModel, TId> idSelector, TModel model, CancellationToken cancellationToken)
        {
            var id = idSelector(model);

            var found = await _context
                .Set<TModel>()
                .FirstOrDefaultAsync(
                    GetFilter(model => idSelector(model)!.Equals(id)), 
                    cancellationToken);

            if (found is null)
                return null;

            _context
                .Set<TModel>()
                .Update(model);

            return found;
        }

        protected override async Task<TModel?> InternalDeleteAsync<TId>(Func<TModel, TId> idSelector, TId id, CancellationToken cancellationToken)
        {
            var found = await _context
                .Set<TModel>()
                .FirstOrDefaultAsync(
                    GetFilter(model => idSelector(model)!.Equals(id)), 
                    cancellationToken);

            if (found is null)
                return null;

            _context
                .Set<TModel>()
                .Remove(found);

            return found;
        }
    }
}
