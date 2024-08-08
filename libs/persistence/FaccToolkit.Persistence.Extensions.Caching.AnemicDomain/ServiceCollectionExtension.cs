using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace FaccToolkit.Persistence.Extensions.Caching.AnemicDomain
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCachedRepositories<TReadRepositoryImplementation, TWriteRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TReadRepositoryImplementation : class, IReadRepository<TEntity, TId>
            where TWriteRepositoryImplementation : class, IWriteRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services
                .AddCachedReadRepository<TReadRepositoryImplementation, TEntity, TId>()
                .AddCachedWriteRepository<TWriteRepositoryImplementation, TEntity, TId>();

        public static IServiceCollection AddCachedReadRepository<TReadRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TReadRepositoryImplementation : class, IReadRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TReadRepositoryImplementation>()
                .AddScoped<IReadRepository<TEntity, TId>, CacheReadRepository<TReadRepositoryImplementation, TEntity, TId>>(provider =>
                    new CacheReadRepository<TReadRepositoryImplementation, TEntity, TId>(
                        provider.GetRequiredService<ICacheFacade>(),
                        provider.GetRequiredService<TReadRepositoryImplementation>(),
                        provider.GetRequiredService<ILogger<CacheReadRepository<TReadRepositoryImplementation, TEntity, TId>>>()));

        public static IServiceCollection AddCachedWriteRepository<TWriteRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TWriteRepositoryImplementation : class, IWriteRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TWriteRepositoryImplementation>()
                .AddScoped<IWriteRepository<TEntity, TId>, CacheWriteRepository<TWriteRepositoryImplementation, TEntity, TId>>(provider =>
                    new CacheWriteRepository<TWriteRepositoryImplementation, TEntity, TId>(
                        provider.GetRequiredService<ICacheFacade>(),
                        provider.GetRequiredService<TWriteRepositoryImplementation>(),
                        provider.GetRequiredService<ILogger<CacheWriteRepository<TWriteRepositoryImplementation, TEntity, TId>>>()));

        public static IServiceCollection AddCachedRepositories<TReadRepositoryService, TCacheReadRepositoryImplementation,TReadRepositoryImplementation, TWriteRepositoryService, TCacheWriteRepositoryImplementation, TWriteRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TReadRepositoryService : class, IReadRepository<TEntity, TId>
            where TCacheReadRepositoryImplementation : class, TReadRepositoryService
            where TReadRepositoryImplementation : class, TReadRepositoryService
            where TWriteRepositoryService : class, IWriteRepository<TEntity, TId>
            where TCacheWriteRepositoryImplementation : class, TWriteRepositoryService
            where TWriteRepositoryImplementation : class, TWriteRepositoryService
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services
                .AddCachedReadRepository<TReadRepositoryService, TCacheReadRepositoryImplementation, TReadRepositoryImplementation, TEntity, TId>()
                .AddCachedWriteRepository<TWriteRepositoryService, TCacheWriteRepositoryImplementation, TWriteRepositoryImplementation, TEntity, TId>();

        public static IServiceCollection AddCachedReadRepository<TReadRepositoryService, TCacheReadRepositoryImplementation, TReadRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TReadRepositoryService : class, IReadRepository<TEntity, TId>
            where TCacheReadRepositoryImplementation : class, TReadRepositoryService
            where TReadRepositoryImplementation : class, TReadRepositoryService
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TReadRepositoryImplementation>()
                .AddScoped<TReadRepositoryService, TCacheReadRepositoryImplementation>();

        public static IServiceCollection AddCachedWriteRepository<TWriteRepositoryService, TCacheWriteRepositoryImplementation, TWriteRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TWriteRepositoryService : class, IWriteRepository<TEntity, TId>
            where TCacheWriteRepositoryImplementation : class, TWriteRepositoryService
            where TWriteRepositoryImplementation : class, TWriteRepositoryService
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TWriteRepositoryImplementation>()
                .AddScoped<TWriteRepositoryService, TCacheWriteRepositoryImplementation>();
    }
}
