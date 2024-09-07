using FaccToolkit.Domain.Rich;
using FaccToolkit.Persistence.EntityFramework.RichDomain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace FaccToolkit.Persistence.EntityFramework.AnemicDomain
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepository<TAggregateRoot, TId, TContext>(this IServiceCollection services)
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            where TContext : DbContext
            => services.AddScoped<IAggregateRepository<TAggregateRoot, TId>, AggregateRepository<TAggregateRoot, TId, TContext>>(provider =>
                new AggregateRepository<TAggregateRoot, TId, TContext>(
                    provider.GetRequiredService<TContext>(),
                    provider.GetRequiredService<ILogger<AggregateRepository<TAggregateRoot, TId, TContext>>>(),
                    provider.GetRequiredService<IDomainEventDispatcher>()));

        public static IServiceCollection AddRepository<TRepositoryImplementation, TAggregateRoot, TId, TContext>(this IServiceCollection services)
            where TRepositoryImplementation : class, IAggregateRepository<TAggregateRoot, TId>
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            where TContext : DbContext
            => services.AddRepository<IAggregateRepository<TAggregateRoot, TId>, TRepositoryImplementation, TAggregateRoot, TId, TContext>();

        public static IServiceCollection AddRepository<TRepositoryService, TRepositoryImplementation, TAggregateRoot, TId, TContext>(this IServiceCollection services)
            where TRepositoryService : class, IAggregateRepository<TAggregateRoot, TId>
            where TRepositoryImplementation : class, TRepositoryService
            where TAggregateRoot : class, IAggregateRoot<TId>
            where TId : IEquatable<TId>
            where TContext : DbContext
            => services.AddScoped<TRepositoryService, TRepositoryImplementation>();
    }
}
