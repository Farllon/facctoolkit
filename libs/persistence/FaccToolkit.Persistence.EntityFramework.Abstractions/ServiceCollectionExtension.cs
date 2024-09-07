using FaccToolkit.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace FaccToolkit.Persistence.EntityFramework.Abstractions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddEfUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.TryAddScoped<IUnitOfWork, EfUnitOfWork<TContext>>();

            return services;
        }
    }
}
