using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace FaccToolkit.Persistence.EntityFramework.Abstractions
{
    public class BaseRepository<TModel, TDbContext> : IAsyncDisposable
        where TModel : class
        where TDbContext : DbContext
    {
        protected readonly TDbContext _context;
        protected readonly ILogger<BaseRepository<TModel, TDbContext>> _logger;

        public BaseRepository(TDbContext context, ILogger<BaseRepository<TModel, TDbContext>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public virtual async Task InsertAsync<TId>(Func<TModel, TId> idSelector, TModel model, CancellationToken cancellationToken)
        {
            var id = idSelector.Invoke(model);

            _logger.LogInformation("Inserting {Model} model with {Id} id in database", typeof(TModel), id);

            try
            {
                await _context.Set<TModel>()
                    .AddAsync(model, cancellationToken)
                    .ConfigureAwait(false);

                _logger.LogInformation("Successfully inserted {Model} model with {Id} id", typeof(TModel), id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert {Model} model with {Id} id", typeof(TModel), id);
                throw;
            }
        }

        public virtual async Task InsertAsync(IEnumerable<TModel> models, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Inserting multiple {Model} models into the database", typeof(TModel));

            try
            {
                await _context.Set<TModel>()
                    .AddRangeAsync(models, cancellationToken)
                    .ConfigureAwait(false);

                _logger.LogInformation("Successfully inserted multiple {Model} models", typeof(TModel));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert multiple {Model} models", typeof(TModel));
                throw;
            }
        }

        public virtual async Task UpdateAsync<TId>(Func<TModel, TId> idSelector, TModel model, CancellationToken cancellationToken)
        {
            var id = idSelector.Invoke(model);

            _logger.LogInformation("Updating {Model} model with {Id} id in database", typeof(TModel), id);

            try
            {
                var found = await _context.Set<TModel>().FindAsync(new object?[] { id }, cancellationToken: cancellationToken);

                if (found is null)
                {
                    _logger.LogWarning("The {Model} model with {Id} id was not found", typeof(TModel), id);

                    return;
                }

                _context.Set<TModel>().Update(model);

                _logger.LogInformation("Successfully updated {Model} model with {Id} id", typeof(TModel), id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update {Model} model with {Id} id", typeof(TModel), id);
                throw;
            }
        }

        public virtual async  Task DeleteAsync<TId>(TId id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting {Model} model with {Id} id from the database", typeof(TModel), id);

            try
            {
                var model = await _context.Set<TModel>().FindAsync(new object?[] { id }, cancellationToken: cancellationToken);

                if (model is null)
                {
                    _logger.LogWarning("The {Model} model with {Id} id was not found", typeof(TModel), id);

                    return;
                }

                _context.Set<TModel>().Remove(model);

                _logger.LogInformation("Successfully deleted {Model} model with {Id} id from the database", typeof(TModel), id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete {Model} model with {Id} id from the database", typeof(TModel), id);
                throw;
            }
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
    }
}
