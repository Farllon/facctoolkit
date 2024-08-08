using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Data;
using FaccToolkit.Persistence.MongoDb.AnemicDomain;
using MongoDB.Driver;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Repositories
{
    public class PostReadRepository : ReadRepository<Post, Guid>, IPostReadRepository
    {
        public PostReadRepository(MyMongoDbReadOnlyContext context, ILogger<PostReadRepository> logger) 
            : base(MongoCollections.Posts, context, logger)
        {
        }

        public Task<IReadOnlyCollection<Post>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding Post entities by author {AuthorId} id in mongo {Collection} collection", authorId, _collection.CollectionNamespace.CollectionName);

            return _collection
                .Find(_context.CurrentSession, Builders<Post>.Filter.Eq(post => post.AuthorId, authorId))
                .ToListAsync(cancellationToken)
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                        _logger.LogInformation("The Post entities with author {AuthorId} id was found in in mongo {Collection} collection", authorId, _collection.CollectionNamespace.CollectionName);
                    else
                        _logger.LogError(task.Exception, "The Post entities find operation was failed");

                    return task.Result as IReadOnlyCollection<Post>;
                });
        }
    }
}
