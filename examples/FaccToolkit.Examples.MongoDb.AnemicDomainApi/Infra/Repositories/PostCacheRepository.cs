using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Persistence.Extensions.Caching.AnemicDomain;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Repositories
{
    public class PostCacheRepository : CacheRepository<PostRepository, Post, Guid>, IPostRepository
    {
        public PostCacheRepository(ICacheFacade cacheFacade, PostRepository dbReadRepository, ILogger<PostCacheRepository> logger)
            : base(cacheFacade, dbReadRepository, logger)
        {

        }

        public Task<IReadOnlyCollection<Post>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken)
            => _dbRepository.GetByAuthorIdAsync(authorId, cancellationToken);
    }
}
