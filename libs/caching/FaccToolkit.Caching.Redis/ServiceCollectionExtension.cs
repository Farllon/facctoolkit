using FaccToolkit.Caching.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FaccToolkit.Caching.Redis
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRedisCaching(this IServiceCollection services, Action<RedisConfiguration> configureCache, Func<IServiceProvider, string>? scopedPerfixFactory = null)
        {
            var cacheConfig = new RedisConfiguration();

            configureCache(cacheConfig);

            services.AddSingleton<IConnectionMultiplexer>(provider =>
                ConnectionMultiplexer.Connect(
                    provider
                        .GetRequiredService<IConfiguration>()
                        .GetConnectionString("Redis")!));

            services.AddSingleton(provider =>
                provider
                    .GetRequiredService<IConnectionMultiplexer>()
                    .GetDatabase(cacheConfig.Database));

            if (scopedPerfixFactory != null)
                services.AddScoped<ICacheFacade, CacheFacade>(provider =>
                    new CacheFacade(
                        provider.GetRequiredService<IModelSerializer>(),
                        provider.GetRequiredService<IDatabase>(),
                        provider.GetRequiredService<ILogger<CacheFacade>>(),
                        cacheConfig,
                        scopedPerfixFactory(provider)));
            else
                services.AddSingleton<ICacheFacade, CacheFacade>(provider => 
                    new CacheFacade(
                        provider.GetRequiredService<IModelSerializer>(),
                        provider.GetRequiredService<IDatabase>(),
                        provider.GetRequiredService<ILogger<CacheFacade>>(),
                        cacheConfig));

            return services;
        }

        public static IServiceCollection AddRedisCaching(this IServiceCollection services, Action<RedisConfiguration> configureCache, Func<IServiceProvider, IDatabase> databaseFactory, Func<IServiceProvider, string>? scopedPerfixFactory = null)
        {
            var cacheConfig = new RedisConfiguration();

            configureCache(cacheConfig);

            if (scopedPerfixFactory != null)
                services.AddScoped<ICacheFacade, CacheFacade>(provider =>
                    new CacheFacade(
                        provider.GetRequiredService<IModelSerializer>(),
                        databaseFactory(provider),
                        provider.GetRequiredService<ILogger<CacheFacade>>(),
                        cacheConfig,
                        scopedPerfixFactory(provider)));
            else
                services.AddSingleton<ICacheFacade, CacheFacade>(provider =>
                    new CacheFacade(
                        provider.GetRequiredService<IModelSerializer>(),
                        databaseFactory(provider),
                        provider.GetRequiredService<ILogger<CacheFacade>>(),
                        cacheConfig,
                        null));

            return services;
        }
    }
}
