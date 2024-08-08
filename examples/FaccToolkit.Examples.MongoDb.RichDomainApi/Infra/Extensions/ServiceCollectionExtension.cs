using FaccToolkit.Domain.Rich.Extensions.MediatR;
using FaccToolkit.Examples.MongoDb.RichDomainApi.Infra.Data;
using FaccToolkit.Examples.MongoDb.RichDomainApi.Infra.Data.Repositories;
using FaccToolkit.Examples.MongoDb.RichDomainApi.Infra.Data.Serializers;
using FaccToolkit.Examples.RichDomain.Aggregations.Authors;
using FaccToolkit.Persistence.MongoDb.Abstractions;
using FaccToolkit.Persistence.MongoDb.RichDomain;
using MongoDB.Bson.Serialization;

namespace FaccToolkit.Examples.MongoDb.RichDomainApi.Infra.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddInfra(this IServiceCollection services)
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

            services.AddRepositories<IAuthorReadRepository, AuthorReadRepository, IAuthorWriteRepository, AuthorWriteRepository, Author, Guid>();

            services.AddMongoDbUnitOfWork<MyMongoDbContext>();

            BsonClassMap.RegisterClassMap<Author>(mapper =>
            {
                mapper.MapProperty(author => author.Name).SetSerializer(NameSerializer.Instance);
                mapper.MapProperty(author => author.Posts);
                mapper.MapCreator(author => new Author(author.Id, author.Name, author.Posts.ToList()));
            });

            BsonClassMap.RegisterClassMap<Post>(mapper =>
            {
                mapper.MapProperty(post => post.Title).SetSerializer(TitleSerializer.Instance);
                mapper.MapProperty(post => post.Content).SetSerializer(ContentSerializer.Instance);
            });

            services.AddMediatRDomainEventDispatcher();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                AppDomain.CurrentDomain.GetAssemblies()));
        }
    }
}
