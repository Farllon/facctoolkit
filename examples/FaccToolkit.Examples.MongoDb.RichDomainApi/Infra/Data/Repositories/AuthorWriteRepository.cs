using FaccToolkit.Domain.Rich;
using FaccToolkit.Examples.RichDomain.Aggregations.Authors;
using FaccToolkit.Persistence.MongoDb.RichDomain;

namespace FaccToolkit.Examples.MongoDb.RichDomainApi.Infra.Data.Repositories
{
    public class AuthorWriteRepository : WriteRepository<Author, Guid>, IAuthorWriteRepository
    {
        public AuthorWriteRepository(MyMongoDbContext context, ILogger<AuthorWriteRepository> logger, IDomainEventDispatcher dispatcher) 
            : base(MongoCollections.Authors, context, logger, dispatcher)
        {
        }
    }
}
