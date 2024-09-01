using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace FaccToolkit.Persistence.MongoDb.Abstractions.Tests
{
    public class TestMongoDbContext : MongoDbContext
    {
        public TestMongoDbContext(IMongoClient client, ILogger<MongoDbContext> logger)
            : base("test-db", client, logger)
        {

        }
    }
}
