using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Data;
using FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Repositories;
using FaccToolkit.Persistence.Extensions.Caching.AnemicDomain;
using FaccToolkit.Persistence.MongoDb.Abstractions;
using FaccToolkit.Caching.Redis;
using FaccToolkit.Caching.Serializers.System.Text.Json;
using System.Reflection;
using FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Converters;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddInfra(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoDbContext<MyMongoDbContext>(
                (provider, client, logger) =>
                {
                    var configurations = provider.GetRequiredService<IConfiguration>();

                    var mongoConfigSection = configurations.GetSection("MongoDBConfigurations");

                    return new MyMongoDbContext(mongoConfigSection.GetValue<string>("DatabaseName")!, client, logger);
                },
                provider =>
                {
                    var configurations = provider.GetRequiredService<IConfiguration>();

                    return configurations.GetConnectionString("MongoDB")!;
                });

            services.AddMongoDbContext<MyMongoDbReadOnlyContext>(
                (provider, client, logger) =>
                {
                    var configurations = provider.GetRequiredService<IConfiguration>();

                    var mongoConfigSection = configurations.GetSection("MongoDBConfigurations");

                    return new MyMongoDbReadOnlyContext(mongoConfigSection.GetValue<string>("DatabaseName")!, client, logger);
                },
                provider =>
                {
                    var configurations = provider.GetRequiredService<IConfiguration>();

                    return configurations.GetConnectionString("MongoDBReadOnly")!;
                });

            services.AddRedisCaching(cacheConfig =>
            {
                cacheConfig.Database = configuration.GetSection("RedisConfiguration").GetSection(nameof(RedisConfiguration.Database)).Get<int>();
                cacheConfig.Prefix = configuration.GetSection("RedisConfiguration").GetSection(nameof(RedisConfiguration.Prefix)).Get<string>()!;
                cacheConfig.SuppressCacheSetErrors = configuration.GetSection("RedisConfiguration").GetSection(nameof(RedisConfiguration.SuppressCacheSetErrors)).Get<bool>();
                cacheConfig.ExpirationInMilliseconds = configuration.GetSection("RedisConfiguration").GetSection(nameof(RedisConfiguration.ExpirationInMilliseconds)).Get<int>();
            }, provider => "Farllon");

            services.AddCacheSerializer(options =>
            {
                options.Converters.Add(new AuthorConverter());
                options.Converters.Add(new PostConverter());
            });

            services.AddCachedRepository<IAuthorRepository, AuthorCacheRepository, AuthorRepository, Author, Guid>();
            services.AddCachedRepository<IPostRepository, PostCacheRepository, PostRepository, Post, Guid>();

            services.AddMongoDbUnitOfWork<MyMongoDbContext>();
        }
    }
}
