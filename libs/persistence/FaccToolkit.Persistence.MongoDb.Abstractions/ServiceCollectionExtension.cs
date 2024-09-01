using FaccToolkit.Persistence.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FaccToolkit.Persistence.MongoDb.Abstractions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddMongoDbContext<TContext>(
            this IServiceCollection services,
            Func<IServiceProvider, IMongoClient, ILogger<TContext>, TContext> contextFactory,
            Func<IServiceProvider, string> connectionStringFactory)
            where TContext : class, IMongoDbContext
        {
            services.AddSingleton<IMongoClient, MongoClient>(provider =>
            {
                var connectionString = connectionStringFactory(provider);

                return new MongoClient(connectionString);
            });

            services.AddScoped(provider =>
            {
                var client = provider.GetRequiredService<IMongoClient>();
                var logger = provider.GetRequiredService<ILogger<TContext>>();

                var context = contextFactory(provider, client, logger);

                return context;
            });

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TContext>(
            this IServiceCollection services,
            Func<IServiceProvider, IMongoClient, ILogger<TContext>, TContext> contextFactory,
            string connectionString)
            where TContext : class, IMongoDbContext
            => AddMongoDbContext(services, contextFactory, _ => connectionString);

        public static IServiceCollection AddMongoDbUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : MongoDbContext
        {
            services.TryAddScoped<IUnitOfWork, MongoDbUnitOfWork<TContext>>();

            return services;
        }
    }
}
