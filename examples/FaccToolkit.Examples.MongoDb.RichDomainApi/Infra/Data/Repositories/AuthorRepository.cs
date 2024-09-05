using FaccToolkit.Domain.Rich;
using FaccToolkit.Examples.RichDomain.Aggregations.Authors;
using FaccToolkit.Persistence.MongoDb.RichDomain;
using MongoDB.Driver;

namespace FaccToolkit.Examples.MongoDb.RichDomainApi.Infra.Data.Repositories
{
    public class AuthorRepository : AggregateRepository<Author, Guid>, IAuthorRepository
    {
        public AuthorRepository(MyMongoDbContext context, ILogger<AuthorRepository> logger, IDomainEventDispatcher dispatcher) 
            : base(MongoCollections.Authors, context, logger, dispatcher)
        {
        }

        public Task<IReadOnlyCollection<Author>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding Author entities in mongo {Collection} collection", _collection.CollectionNamespace.CollectionName);

            return _collection
                .Find(_context.CurrentSession, Builders<Author>.Filter.Empty)
                .ToListAsync(cancellationToken)
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                        _logger.LogInformation("The Author entities was found in in mongo {Collection} collection", _collection.CollectionNamespace.CollectionName);
                    else
                        _logger.LogError(task.Exception, "The Author entities find operation was failed");

                    return task.Result as IReadOnlyCollection<Author>;
                });
        }
    }
}
