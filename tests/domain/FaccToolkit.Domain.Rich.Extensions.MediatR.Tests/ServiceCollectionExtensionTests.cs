using Microsoft.Extensions.DependencyInjection;

namespace FaccToolkit.Domain.Rich.Extensions.MediatR.Tests
{
    public class ServiceCollectionExtensionTests
    {
        [Fact]
        public void WhenCallAddMediatRDomainEventDispatcher_ShouldAddServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMediatRDomainEventDispatcher();

            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(IDomainEventDispatcher) && s.ImplementationType == typeof(DomainEventDispatcher));
        }
    }
}
