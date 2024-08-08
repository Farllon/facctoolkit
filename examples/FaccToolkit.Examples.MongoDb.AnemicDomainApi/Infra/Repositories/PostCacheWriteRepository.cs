using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Persistence.Extensions.Caching.AnemicDomain;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Repositories
{
    public class PostCacheWriteRepository : CacheWriteRepository<PostWriteRepository, Post, Guid>, IPostWriteRepository
    {
        public PostCacheWriteRepository(ICacheFacade cacheFacade, PostWriteRepository dbReadRepository, ILogger<PostCacheWriteRepository> logger)
            : base(cacheFacade, dbReadRepository, logger)
        {

        }
    }
}
