using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Persistence.Extensions.Caching.AnemicDomain;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Repositories
{
    public class AuthorCacheReadRepository : CacheReadRepository<AuthorReadRepository, Author, Guid>, IAuthorReadRepository
    {
        public AuthorCacheReadRepository(ICacheFacade cacheFacade, AuthorReadRepository dbReadRepository, ILogger<AuthorCacheReadRepository> logger) 
            : base(cacheFacade, dbReadRepository, logger)
        {

        }

        public Task<IReadOnlyCollection<Author>> GetAllAsync(CancellationToken cancellationToken)
            => _dbReadRepository.GetAllAsync(cancellationToken);
    }
}
