using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Domain.Rich;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace FaccToolkit.Persistence.Extensions.Caching.RichDomain
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCachedRepositories<TReadRepositoryImplementation, TWriteRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TReadRepositoryImplementation : class, IReadRepository<TAggregateRoot, TId>
            where TWriteRepositoryImplementation : class, IWriteRepository<TAggregateRoot, TId>
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services
                .AddCachedReadRepository<TReadRepositoryImplementation, TAggregateRoot, TId>()
                .AddCachedWriteRepository<TWriteRepositoryImplementation, TAggregateRoot, TId>();

        public static IServiceCollection AddCachedReadRepository<TReadRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TReadRepositoryImplementation : class, IReadRepository<TAggregateRoot, TId>
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TReadRepositoryImplementation>()
                .AddScoped<IReadRepository<TAggregateRoot, TId>, CacheReadRepository<TReadRepositoryImplementation, TAggregateRoot, TId>>(provider =>
                    new CacheReadRepository<TReadRepositoryImplementation, TAggregateRoot, TId>(
                        provider.GetRequiredService<ICacheFacade>(),
                        provider.GetRequiredService<TReadRepositoryImplementation>(),
                        provider.GetRequiredService<ILogger<CacheReadRepository<TReadRepositoryImplementation, TAggregateRoot, TId>>>()));

        public static IServiceCollection AddCachedWriteRepository<TWriteRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TWriteRepositoryImplementation : class, IWriteRepository<TAggregateRoot, TId>
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TWriteRepositoryImplementation>()
                .AddScoped<IWriteRepository<TAggregateRoot, TId>, CacheWriteRepository<TWriteRepositoryImplementation, TAggregateRoot, TId>>(provider =>
                    new CacheWriteRepository<TWriteRepositoryImplementation, TAggregateRoot, TId>(
                        provider.GetRequiredService<ICacheFacade>(),
                        provider.GetRequiredService<TWriteRepositoryImplementation>(),
                        provider.GetRequiredService<ILogger<CacheWriteRepository<TWriteRepositoryImplementation, TAggregateRoot, TId>>>()));

        public static IServiceCollection AddCachedRepositories<TReadRepositoryService, TCacheReadRepositoryImplementation, TReadRepositoryImplementation, TWriteRepositoryService, TCacheWriteRepositoryImplementation, TWriteRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TReadRepositoryService : class, IReadRepository<TAggregateRoot, TId>
            where TCacheReadRepositoryImplementation : class, TReadRepositoryService
            where TReadRepositoryImplementation : class, TReadRepositoryService
            where TWriteRepositoryService : class, IWriteRepository<TAggregateRoot, TId>
            where TCacheWriteRepositoryImplementation : class, TWriteRepositoryService
            where TWriteRepositoryImplementation : class, TWriteRepositoryService
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services
                .AddCachedReadRepository<TReadRepositoryService, TCacheReadRepositoryImplementation, TReadRepositoryImplementation, TAggregateRoot, TId>()
                .AddCachedWriteRepository<TWriteRepositoryService, TCacheWriteRepositoryImplementation, TWriteRepositoryImplementation, TAggregateRoot, TId>();

        public static IServiceCollection AddCachedReadRepository<TReadRepositoryService, TCacheReadRepositoryImplementation, TReadRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TReadRepositoryService : class, IReadRepository<TAggregateRoot, TId>
            where TCacheReadRepositoryImplementation : class, TReadRepositoryService
            where TReadRepositoryImplementation : class, TReadRepositoryService
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TReadRepositoryImplementation>()
                .AddScoped<TReadRepositoryService, TCacheReadRepositoryImplementation>();

        public static IServiceCollection AddCachedWriteRepository<TWriteRepositoryService, TCacheWriteRepositoryImplementation, TWriteRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TWriteRepositoryService : class, IWriteRepository<TAggregateRoot, TId>
            where TCacheWriteRepositoryImplementation : class, TWriteRepositoryService
            where TWriteRepositoryImplementation : class, TWriteRepositoryService
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services
                .AddScoped<TWriteRepositoryImplementation>()
                .AddScoped<TWriteRepositoryService, TCacheWriteRepositoryImplementation>();
    }
}
