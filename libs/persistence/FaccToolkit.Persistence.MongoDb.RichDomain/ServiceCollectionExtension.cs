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
        public static IServiceCollection AddRepositories<TAggregateRoot, TId, TContext>(this IServiceCollection services, string? collectionName = null)
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            where TContext : class, IMongoDbContext
            => services
                .AddReadRepository<TAggregateRoot, TId, TContext>(collectionName)
                .AddWriteRepository<TAggregateRoot, TId, TContext>(collectionName);

        public static IServiceCollection AddReadRepository<TAggregateRoot, TId, TContext>(this IServiceCollection services, string? collectionName = null)
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            where TContext : class, IMongoDbContext
            => services.AddScoped<IReadRepository<TAggregateRoot, TId>, ReadRepository<TAggregateRoot, TId>>(provider =>
                new ReadRepository<TAggregateRoot, TId>(
                    collectionName ?? typeof(TAggregateRoot).Name,
                    provider.GetRequiredService<TContext>(),
                    provider.GetRequiredService<ILogger<ReadRepository<TAggregateRoot, TId>>>()));

        public static IServiceCollection AddWriteRepository<TAggregateRoot, TId, TContext>(this IServiceCollection services, string? collectionName = null)
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            where TContext : class, IMongoDbContext
            => services.AddScoped<IWriteRepository<TAggregateRoot, TId>, WriteRepository<TAggregateRoot, TId>>(provider =>
                new WriteRepository<TAggregateRoot, TId>(
                    collectionName ?? typeof(TAggregateRoot).Name,
                    provider.GetRequiredService<TContext>(),
                    provider.GetRequiredService<ILogger<WriteRepository<TAggregateRoot, TId>>>(),
                    provider.GetRequiredService<IDomainEventDispatcher>()));

        public static IServiceCollection AddRepositories<TReadRepositoryImplementation, TWriteRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TReadRepositoryImplementation : class, IReadRepository<TAggregateRoot, TId>
            where TWriteRepositoryImplementation : class, IWriteRepository<TAggregateRoot, TId>
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services
                .AddReadRepository<TReadRepositoryImplementation, TAggregateRoot, TId>()
                .AddWriteRepository<TWriteRepositoryImplementation, TAggregateRoot, TId>();

        public static IServiceCollection AddReadRepository<TReadRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TReadRepositoryImplementation : class, IReadRepository<TAggregateRoot, TId>
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services.AddReadRepository<IReadRepository<TAggregateRoot, TId>, TReadRepositoryImplementation, TAggregateRoot, TId>();

        public static IServiceCollection AddWriteRepository<TWriteRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TWriteRepositoryImplementation : class, IWriteRepository<TAggregateRoot, TId>
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services.AddWriteRepository<IWriteRepository<TAggregateRoot, TId>, TWriteRepositoryImplementation, TAggregateRoot, TId>();

        public static IServiceCollection AddRepositories<TReadRepositoryService, TReadRepositoryImplementation, TWriteRepositoryService, TWriteRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TReadRepositoryService : class, IReadRepository<TAggregateRoot, TId>
            where TReadRepositoryImplementation : class, TReadRepositoryService
            where TWriteRepositoryService : class, IWriteRepository<TAggregateRoot, TId>
            where TWriteRepositoryImplementation : class, TWriteRepositoryService
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services
                .AddReadRepository<TReadRepositoryService, TReadRepositoryImplementation, TAggregateRoot, TId>()
                .AddWriteRepository<TWriteRepositoryService, TWriteRepositoryImplementation, TAggregateRoot, TId>();

        public static IServiceCollection AddReadRepository<TReadRepositoryService, TReadRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TReadRepositoryService : class, IReadRepository<TAggregateRoot, TId>
            where TReadRepositoryImplementation : class, TReadRepositoryService
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services.AddScoped<TReadRepositoryService, TReadRepositoryImplementation>();

        public static IServiceCollection AddWriteRepository<TWriteRepositoryService, TWriteRepositoryImplementation, TAggregateRoot, TId>(this IServiceCollection services)
            where TWriteRepositoryService : class, IWriteRepository<TAggregateRoot, TId>
            where TWriteRepositoryImplementation : class, TWriteRepositoryService
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            => services.AddScoped<TWriteRepositoryService, TWriteRepositoryImplementation>();
    }
}
