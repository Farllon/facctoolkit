using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FaccToolkit.Persistence.MongoDb.AnemicDomain
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepository<TEntity, TId, TContext>(this IServiceCollection services, string? collectionName = null)
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            where TContext : class, IMongoDbContext
            => services.AddScoped<IEntityRepository<TEntity, TId>, EntityRepository<TEntity, TId>>(provider =>
                new EntityRepository<TEntity, TId>(
                    collectionName ?? typeof(TEntity).Name,
                    provider.GetRequiredService<TContext>(),
                    provider.GetRequiredService<ILogger<EntityRepository<TEntity, TId>>>()));

        public static IServiceCollection AddRepository<TRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TRepositoryImplementation : class, IEntityRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services.AddRepository<IEntityRepository<TEntity, TId>, TRepositoryImplementation, TEntity, TId>();

        public static IServiceCollection AddRepository<TRepositoryService, TRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TRepositoryService : class, IEntityRepository<TEntity, TId>
            where TRepositoryImplementation : class, TRepositoryService
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services.AddScoped<TRepositoryService, TRepositoryImplementation>();
    }
}
