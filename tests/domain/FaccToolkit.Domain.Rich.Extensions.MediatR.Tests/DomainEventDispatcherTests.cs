using MediatR;
using Moq;

namespace FaccToolkit.Domain.Rich.Extensions.MediatR.Tests
{
    public class DomainEventDispatcherTests
    {
        [Fact]
        public async Task DispatchAsync_CallsMediatorPublishWithCorrectParameters()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var dispatcher = new DomainEventDispatcher(mediatorMock.Object);
            var domainEvent = new Mock<IDomainEvent>().Object;
            var cancellationToken = new CancellationToken();

            // Act
            await dispatcher.DispatchAsync(domainEvent, cancellationToken);

            // Assert
            mediatorMock.Verify(m => m.Publish(domainEvent, cancellationToken), Times.Once);
        }
    }
}