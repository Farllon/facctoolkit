using FaccToolkit.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;

namespace FaccToolkit.Caching.Serializers.System.Text.Json
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCacheSerializer(this IServiceCollection services, Action<JsonSerializerOptions>? configureSerializerOptions = null)
        {
            if (configureSerializerOptions != null)
                services
                    .AddOptions<JsonSerializerOptions>()
                    .Configure(configureSerializerOptions);

            services.AddSingleton<IModelSerializer, ModelSerializer>();

            return services;
        }
    }
}
