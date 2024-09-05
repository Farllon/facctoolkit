using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FaccToolkit.Persistence.Extensions.Caching.AnemicDomain
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCachedRepository<TRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TRepositoryImplementation : class, IEntityRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TRepositoryImplementation>()
                .AddScoped<IEntityRepository<TEntity, TId>, CacheRepository<TRepositoryImplementation, TEntity, TId>>(provider =>
                    new CacheRepository<TRepositoryImplementation, TEntity, TId>(
                        provider.GetRequiredService<ICacheFacade>(),
                        provider.GetRequiredService<TRepositoryImplementation>(),
                        provider.GetRequiredService<ILogger<CacheRepository<TRepositoryImplementation, TEntity, TId>>>()));

        public static IServiceCollection AddCachedRepository<TRepositoryService, TCacheRepositoryImplementation, TRepositoryImplementation, TEntity, TId>(this IServiceCollection services)
            where TRepositoryService : class, IEntityRepository<TEntity, TId>
            where TCacheRepositoryImplementation : class, TRepositoryService
            where TRepositoryImplementation : class, TRepositoryService
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TRepositoryImplementation>()
                .AddScoped<TRepositoryService, TCacheRepositoryImplementation>();
    }
}
