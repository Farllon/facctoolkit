using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Data;
using FaccToolkit.Persistence.MongoDb.AnemicDomain;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Repositories
{
    public class PostWriteRepository : WriteRepository<Post, Guid>, IPostWriteRepository
    {
        public PostWriteRepository(MyMongoDbContext context, ILogger<PostWriteRepository> logger) 
            : base(MongoCollections.Posts, context, logger)
        {
        }
    }
}
