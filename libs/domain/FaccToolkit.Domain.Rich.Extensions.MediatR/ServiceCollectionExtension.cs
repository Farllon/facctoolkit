using Microsoft.Extensions.DependencyInjection;

namespace FaccToolkit.Domain.Rich.Extensions.MediatR
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddMediatRDomainEventDispatcher(this IServiceCollection services)
        {
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            return services;
        }
    }
}
