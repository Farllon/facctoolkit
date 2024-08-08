using FaccToolkit.Persistence.MongoDb.Abstractions;
using MongoDB.Driver;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Data
{
    public class MyMongoDbReadOnlyContext : MongoDbContext
    {
        public MyMongoDbReadOnlyContext(string databaseName, IMongoClient client, ILogger<MyMongoDbReadOnlyContext> logger) 
            : base(databaseName, client, logger)
        {

        }
    }
}
