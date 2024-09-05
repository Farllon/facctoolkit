using FaccToolkit.Domain.Rich;
using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FaccToolkit.Persistence.MongoDb.RichDomain
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepository<TAggregateRoot, TId, TContext>(this IServiceCollection services, string? collectionName = null)
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            where TContext : class, IMongoDbContext
            => services.AddScoped<IAggregateRepository<TAggregateRoot, TId>, AggregateRepository<TAggregateRoot, TId>>(provider =>
                new AggregateRepository<TAggregateRoot, TId>(
                    collectionName ?? typeof(TAggregateRoot).Name,
                    provider.GetRequiredService<TContext>(),
                    provider.GetRequiredService<ILogger<AggregateRepository<TAggregateRoot, TId>>>(),
                    provider.GetRequiredService<IDomainEventDispatcher>()));

        public static IServiceCollection AddRepository<TRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TRepositoryImplementation : class, IAggregateRepository<TAggregateRoot, TId>
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services.AddRepository<IAggregateRepository<TAggregateRoot, TId>, TRepositoryImplementation, TAggregateRoot, TId>();

        public static IServiceCollection AddRepository<TRepositoryService, TRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TRepositoryService : class, IAggregateRepository<TAggregateRoot, TId>
            where TRepositoryImplementation : class, TRepositoryService
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services.AddScoped<TRepositoryService, TRepositoryImplementation>();
    }
}
