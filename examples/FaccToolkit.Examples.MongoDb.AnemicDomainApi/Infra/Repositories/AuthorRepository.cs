using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Data;
using FaccToolkit.Persistence.MongoDb.AnemicDomain;
using MongoDB.Driver;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Repositories
{
    public class AuthorRepository : EntityRepository<Author, Guid>, IAuthorRepository
    {
        public AuthorRepository(MyMongoDbContext context, ILogger<AuthorRepository> logger) 
            : base(MongoCollections.Authors, context, logger)
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

        protected override FilterDefinition<Author>? GetQueryFilter()
        {
            return Builders<Author>.Filter.Eq(a => a.Name, "Farllon");
        }
    }
}
