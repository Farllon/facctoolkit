﻿using FaccToolkit.Persistence.MongoDb.Abstractions;
using MongoDB.Driver;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Data
{
    public class MyMongoDbContext : MongoDbContext
    {
        public MyMongoDbContext(string databaseName, IMongoClient client, ILogger<MyMongoDbContext> logger) 
            : base(databaseName, client, logger)
        {
        }
    }
}
