using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Data;
using FaccToolkit.Persistence.MongoDb.AnemicDomain;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Repositories
{
    public class AuthorWriteRepository : WriteRepository<Author, Guid>, IAuthorWriteRepository
    {
        public AuthorWriteRepository(MyMongoDbContext context, ILogger<AuthorWriteRepository> logger) 
            : base(MongoCollections.Authors, context, logger)
        {
        }
    }
}
