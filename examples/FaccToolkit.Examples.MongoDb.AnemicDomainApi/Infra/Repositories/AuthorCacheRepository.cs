using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Persistence.Extensions.Caching.AnemicDomain;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Repositories
{
    public class AuthorCacheRepository : CacheRepository<AuthorRepository, Author, Guid>, IAuthorRepository
    {
        public AuthorCacheRepository(ICacheFacade cacheFacade, AuthorRepository dbReadRepository, ILogger<AuthorCacheRepository> logger)
            : base(cacheFacade, dbReadRepository, logger)
        {

        }

        public Task<IReadOnlyCollection<Author>> GetAllAsync(CancellationToken cancellationToken)
            => _dbRepository.GetAllAsync(cancellationToken);
    }
}
