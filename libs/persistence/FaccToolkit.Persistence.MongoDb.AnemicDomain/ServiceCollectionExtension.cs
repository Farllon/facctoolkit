using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace FaccToolkit.Persistence.MongoDb.AnemicDomain
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepositories<TEntity, TId, TContext>(this IServiceCollection services, string? collectionName = null)
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            where TContext : class, IMongoDbContext
            => services
                .AddReadRepository<TEntity, TId, TContext>(collectionName)
                .AddWriteRepository<TEntity, TId, TContext>(collectionName);

        public static IServiceCollection AddReadRepository<TEntity, TId, TContext>(this IServiceCollection services, string? collectionName = null)
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            where TContext : class, IMongoDbContext
            => services.AddScoped<IReadRepository<TEntity, TId>, ReadRepository<TEntity, TId>>(provider =>
                new ReadRepository<TEntity, TId>(
                    collectionName ?? typeof(TEntity).Name,
                    provider.GetRequiredService<TContext>(),
                    provider.GetRequiredService<ILogger<ReadRepository<TEntity, TId>>>()));

        public static IServiceCollection AddWriteRepository<TEntity, TId, TContext>(this IServiceCollection services, string? collectionName = null)
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            where TContext : class, IMongoDbContext
            => services.AddScoped<IWriteRepository<TEntity, TId>, WriteRepository<TEntity, TId>>(provider =>
                new WriteRepository<TEntity, TId>(
                    collectionName ?? typeof(TEntity).Name,
                    provider.GetRequiredService<TContext>(),
                    provider.GetRequiredService<ILogger<WriteRepository<TEntity, TId>>>()));

        public static IServiceCollection AddRepositories<TReadRepositoryImplementation, TWriteRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TReadRepositoryImplementation : class, IReadRepository<TEntity, TId>
            where TWriteRepositoryImplementation : class, IWriteRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services
                .AddReadRepository<TReadRepositoryImplementation, TEntity, TId>()
                .AddWriteRepository<TWriteRepositoryImplementation, TEntity, TId>();

        public static IServiceCollection AddReadRepository<TReadRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TReadRepositoryImplementation : class, IReadRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services.AddReadRepository<IReadRepository<TEntity, TId>, TReadRepositoryImplementation, TEntity, TId>();

        public static IServiceCollection AddWriteRepository<TWriteRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TWriteRepositoryImplementation : class, IWriteRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services.AddWriteRepository<IWriteRepository<TEntity, TId>, TWriteRepositoryImplementation, TEntity, TId>();

        public static IServiceCollection AddRepositories<TReadRepositoryService, TReadRepositoryImplementation, TWriteRepositoryService, TWriteRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TReadRepositoryService : class, IReadRepository<TEntity, TId>
            where TReadRepositoryImplementation : class, TReadRepositoryService
            where TWriteRepositoryService : class, IWriteRepository<TEntity, TId>
            where TWriteRepositoryImplementation : class, TWriteRepositoryService
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services
                .AddReadRepository<TReadRepositoryService, TReadRepositoryImplementation, TEntity, TId>()
                .AddWriteRepository<TWriteRepositoryService, TWriteRepositoryImplementation, TEntity, TId>();

        public static IServiceCollection AddReadRepository<TReadRepositoryService, TReadRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TReadRepositoryService : class, IReadRepository<TEntity, TId>
            where TReadRepositoryImplementation : class, TReadRepositoryService
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services.AddScoped<TReadRepositoryService, TReadRepositoryImplementation>();

        public static IServiceCollection AddWriteRepository<TWriteRepositoryService, TWriteRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TWriteRepositoryService : class, IWriteRepository<TEntity, TId>
            where TWriteRepositoryImplementation : class, TWriteRepositoryService
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services.AddScoped<TWriteRepositoryService, TWriteRepositoryImplementation>();
    }
}
