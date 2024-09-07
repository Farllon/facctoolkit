using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.EntityFramework.AnemicDomain
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepository<TEntity, TId, TContext>(this IServiceCollection services)
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            where TContext : DbContext
            => services.AddScoped<IEntityRepository<TEntity, TId>, EntityRepository<TEntity, TId, TContext>>(provider =>
                new EntityRepository<TEntity, TId, TContext>(
                    provider.GetRequiredService<TContext>(),
                    provider.GetRequiredService<ILogger<EntityRepository<TEntity, TId, TContext>>>()));

        public static IServiceCollection AddRepository<TRepositoryImplementation, TEntity, TId, TContext>(this IServiceCollection services)
            where TRepositoryImplementation : class, IEntityRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            where TContext : DbContext
            => services.AddRepository<IEntityRepository<TEntity, TId>, TRepositoryImplementation, TEntity, TId, TContext>();

        public static IServiceCollection AddRepository<TRepositoryService, TRepositoryImplementation, TEntity, TId, TContext>(this IServiceCollection services)
            where TRepositoryService : class, IEntityRepository<TEntity, TId>
            where TRepositoryImplementation : class, TRepositoryService
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            where TContext : DbContext
            => services.AddScoped<TRepositoryService, TRepositoryImplementation>();
    }
}
