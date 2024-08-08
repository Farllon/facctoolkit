using FaccToolkit.Caching.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace FaccToolkit.Caching.Redis
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRedisCaching(this IServiceCollection services, Action<RedisConfiguration> configureCache)
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

            services.AddSingleton<ICacheFacade, CacheFacade>(provider => 
                new CacheFacade(
                    provider.GetRequiredService<IModelSerializer>(),
                    provider.GetRequiredService<IDatabase>(),
                    provider.GetRequiredService<ILogger<CacheFacade>>(),
                    cacheConfig));

            return services;
        }

        public static IServiceCollection AddRedisCaching(this IServiceCollection services, Action<RedisConfiguration> configureCache, Func<IServiceProvider, IDatabase> databaseFactory)
        {
            var cacheConfig = new RedisConfiguration();

            configureCache(cacheConfig);

            services.AddSingleton<ICacheFacade, CacheFacade>(provider =>
                new CacheFacade(
                    provider.GetRequiredService<IModelSerializer>(),
                    databaseFactory(provider),
                    provider.GetRequiredService<ILogger<CacheFacade>>(),
                    cacheConfig));

            return services;
        }
    }
}
