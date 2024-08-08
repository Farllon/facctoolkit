using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Persistence.Extensions.Caching.AnemicDomain;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Repositories
{
    public class AuthorCacheWriteRepository : CacheWriteRepository<AuthorWriteRepository, Author, Guid>, IAuthorWriteRepository
    {
        public AuthorCacheWriteRepository(ICacheFacade cacheFacade, AuthorWriteRepository dbReadRepository, ILogger<AuthorCacheWriteRepository> logger)
            : base(cacheFacade, dbReadRepository, logger)
        {

        }
    }
}
