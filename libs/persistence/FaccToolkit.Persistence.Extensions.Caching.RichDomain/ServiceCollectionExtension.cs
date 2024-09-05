using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Domain.Rich;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace FaccToolkit.Persistence.Extensions.Caching.RichDomain
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCachedRepository<TRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TRepositoryImplementation : class, IAggregateRepository<TAggregateRoot, TId>
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TRepositoryImplementation>()
                .AddScoped<IAggregateRepository<TAggregateRoot, TId>, CacheRepository<TRepositoryImplementation, TAggregateRoot, TId>>(provider =>
                    new CacheRepository<TRepositoryImplementation, TAggregateRoot, TId>(
                        provider.GetRequiredService<ICacheFacade>(),
                        provider.GetRequiredService<TRepositoryImplementation>(),
                        provider.GetRequiredService<ILogger<CacheRepository<TRepositoryImplementation, TAggregateRoot, TId>>>()));

        public static IServiceCollection AddCachedRepository<TRepositoryService, TCacheRepositoryImplementation, TRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TRepositoryService : class, IAggregateRepository<TAggregateRoot, TId>
            where TCacheRepositoryImplementation : class, TRepositoryService
            where TRepositoryImplementation : class, TRepositoryService
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TRepositoryImplementation>()
                .AddScoped<TRepositoryService, TCacheRepositoryImplementation>();
    }
}
