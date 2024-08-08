using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Persistence.Extensions.Caching.AnemicDomain;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Repositories
{
    public class PostCacheReadRepository : CacheReadRepository<PostReadRepository, Post, Guid>, IPostReadRepository
    {
        public PostCacheReadRepository(ICacheFacade cacheFacade, PostReadRepository dbReadRepository, ILogger<PostCacheReadRepository> logger)
            : base(cacheFacade, dbReadRepository, logger)
        {

        }

        public Task<IReadOnlyCollection<Post>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken)
            => _dbReadRepository.GetByAuthorIdAsync(authorId, cancellationToken);
    }
}
