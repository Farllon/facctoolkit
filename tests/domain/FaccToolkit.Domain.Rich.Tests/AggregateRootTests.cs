using Moq;

namespace FaccToolkit.Domain.Rich.Tests
{
    public class AggregateRootTests
    {
        public class TestAggregateRoot : AggregateRoot<Guid>
        {
            public TestAggregateRoot(Guid id) : base(id) { }
        }

        [Fact]
        public void EnqueueEvent_AddsEventToList()
        {
            // Arrange
            var id = Guid.NewGuid();
            var aggregateRoot = new TestAggregateRoot(id);
            var domainEvent = new Mock<IDomainEvent>().Object;

            // Act
            aggregateRoot.EnqueueEvent(domainEvent);

            // Assert
            Assert.Contains(domainEvent, aggregateRoot.Events);
        }

        [Fact]
        public void ClearEvents_RemovesAllEvents()
        {
            // Arrange
            var id = Guid.NewGuid();
            var aggregateRoot = new TestAggregateRoot(id);
            var domainEvent = new Mock<IDomainEvent>().Object;
            aggregateRoot.EnqueueEvent(domainEvent);

            // Act
            aggregateRoot.ClearEvents();

            // Assert
            Assert.Empty(aggregateRoot.Events);
        }

        [Fact]
        public async Task DispatchAsync_DispatchesAllEvents()
        {
            // Arrange
            var id = Guid.NewGuid();
            var aggregateRoot = new TestAggregateRoot(id);
            var domainEvent = new Mock<IDomainEvent>().Object;
            var dispatcherMock = new Mock<IDomainEventDispatcher>();
            aggregateRoot.EnqueueEvent(domainEvent);

            // Act
            await aggregateRoot.DispatchAsync(dispatcherMock.Object, CancellationToken.None);

            // Assert
            dispatcherMock.Verify(d => d.DispatchAsync(domainEvent, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Constructor_InitializesId()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var aggregateRoot = new TestAggregateRoot(id);

            // Assert
            Assert.Equal(id, aggregateRoot.Id);
        }
    }
}