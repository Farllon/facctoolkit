using FaccToolkit.Domain.Rich;
using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using System.Linq.Expressions;

namespace FaccToolkit.Persistence.MongoDb.RichDomain.Tests
{
    public class WriteRepositoryTests
    {
        [Fact]
        public async Task WhenInsert_ShouldCallModelRepositoryInsertAndDispatchEvents()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestAggregate>>();
            var modelRepositoryMock = new Mock<IModelRepository<TestAggregate>>();
            var dispatcherMock = new Mock<IDomainEventDispatcher>();
            var entity = new TestAggregate();
            entity.Created();

            var eventsCount = entity.Events.Count;

            contextMock
                .Setup(context => context.GetCollection<TestAggregate, Guid>(It.IsAny<string>()))
                .Returns(collectionMock);

            contextMock
                .Setup(context => context.GetModelRepository<TestAggregate>(
                    It.IsAny<string>(),
                    It.IsAny<ILogger>()))
                .Returns(modelRepositoryMock.Object);

            modelRepositoryMock
                .Setup(modelRepository => modelRepository.InsertAsync(
                    It.IsAny<Expression<Func<TestAggregate, Guid>>>(),
                    entity,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            dispatcherMock
                .Setup(dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var repository = new WriteRepository<TestAggregate, Guid>(
                "test-collection",
                contextMock.Object,
                Mock.Of<ILogger<WriteRepository<TestAggregate, Guid>>>(),
                dispatcherMock.Object);

            await repository.InsertAsync(entity, CancellationToken.None);

            modelRepositoryMock.Verify(
                modelRepository => modelRepository.InsertAsync(
                    It.IsAny<Expression<Func<TestAggregate, Guid>>>(),
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            dispatcherMock.Verify(
                dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(eventsCount));
        }

        [Fact]
        public async Task WhenInsertMany_ShouldCallModelRepositoryInsertManyAndDispatchEvents()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestAggregate>>();
            var modelRepositoryMock = new Mock<IModelRepository<TestAggregate>>();
            var dispatcherMock = new Mock<IDomainEventDispatcher>();
            var entity = new TestAggregate();
            entity.Created();
            var entities = new List<TestAggregate>
            {
                entity
            };
            var eventsCount = entity.Events.Count;

            contextMock
                .Setup(context => context.GetCollection<TestAggregate, Guid>(It.IsAny<string>()))
                .Returns(collectionMock);

            contextMock
                .Setup(context => context.GetModelRepository<TestAggregate>(
                    It.IsAny<string>(),
                    It.IsAny<ILogger>()))
                .Returns(modelRepositoryMock.Object);

            modelRepositoryMock
                .Setup(modelRepository => modelRepository.InsertAsync(
                    It.IsAny<Expression<Func<TestAggregate, Guid>>>(),
                    entities,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            dispatcherMock
                .Setup(dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var repository = new WriteRepository<TestAggregate, Guid>(
                "test-collection",
                contextMock.Object,
                Mock.Of<ILogger<WriteRepository<TestAggregate, Guid>>>(),
                dispatcherMock.Object);

            await repository.InsertAsync(entities, CancellationToken.None);

            modelRepositoryMock.Verify(
                modelRepository => modelRepository.InsertAsync(
                    It.IsAny<Expression<Func<TestAggregate, Guid>>>(),
                    entities,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            dispatcherMock.Verify(
                dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(eventsCount));
        }

        [Fact]
        public async Task WhenUpdate_ShouldCallModelRepositoryUpdateAndDispatchEvents()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestAggregate>>();
            var modelRepositoryMock = new Mock<IModelRepository<TestAggregate>>();
            var dispatcherMock = new Mock<IDomainEventDispatcher>();
            var entity = new TestAggregate();
            entity.Updated();
            var eventsCount = entity.Events.Count;

            contextMock
                .Setup(context => context.GetCollection<TestAggregate, Guid>(It.IsAny<string>()))
                .Returns(collectionMock);

            contextMock
                .Setup(context => context.GetModelRepository<TestAggregate>(
                    It.IsAny<string>(),
                    It.IsAny<ILogger>()))
                .Returns(modelRepositoryMock.Object);

            modelRepositoryMock
                .Setup(modelRepository => modelRepository.UpdateAsync(
                    It.IsAny<Expression<Func<TestAggregate, Guid>>>(),
                    entity,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestAggregate?>(entity));

            dispatcherMock
                .Setup(dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var repository = new WriteRepository<TestAggregate, Guid>(
                "test-collection",
                contextMock.Object,
                Mock.Of<ILogger<WriteRepository<TestAggregate, Guid>>>(),
                dispatcherMock.Object);

            await repository.UpdateAsync(entity, CancellationToken.None);

            modelRepositoryMock.Verify(
                modelRepository => modelRepository.UpdateAsync(
                    It.IsAny<Expression<Func<TestAggregate, Guid>>>(),
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            dispatcherMock.Verify(
                dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(eventsCount));
        }

        [Fact]
        public async Task WhenDelete_ShouldCallModelRepositoryDeleteAndDispatchEvents()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestAggregate>>();
            var modelRepositoryMock = new Mock<IModelRepository<TestAggregate>>();
            var dispatcherMock = new Mock<IDomainEventDispatcher>();
            var entity = new TestAggregate();
            entity.Deleted();
            var eventsCount = entity.Events.Count;

            contextMock
                .Setup(context => context.GetCollection<TestAggregate, Guid>(It.IsAny<string>()))
                .Returns(collectionMock);

            contextMock
                .Setup(context => context.GetModelRepository<TestAggregate>(
                    It.IsAny<string>(),
                    It.IsAny<ILogger>()))
                .Returns(modelRepositoryMock.Object);

            modelRepositoryMock
                .Setup(modelRepository => modelRepository.DeleteAsync(
                    It.IsAny<Expression<Func<TestAggregate, Guid>>>(),
                    entity.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestAggregate?>(entity));

            dispatcherMock
                .Setup(dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var repository = new WriteRepository<TestAggregate, Guid>(
                "test-collection",
                contextMock.Object,
                Mock.Of<ILogger<WriteRepository<TestAggregate, Guid>>>(),
                dispatcherMock.Object);

            await repository.DeleteAsync(entity, CancellationToken.None);

            modelRepositoryMock.Verify(
                modelRepository => modelRepository.DeleteAsync(
                    It.IsAny<Expression<Func<TestAggregate, Guid>>>(),
                    entity.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            dispatcherMock.Verify(
                dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(eventsCount));
        }

        [Fact]
        public async Task WhenDispatchingEvent_ShouldCallDispatcherDispatchEvents()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestAggregate>>();
            var modelRepositoryMock = new Mock<IModelRepository<TestAggregate>>();
            var dispatcherMock = new Mock<IDomainEventDispatcher>();
            var loggerMock = new Mock<ILogger<WriteRepository<TestAggregate, Guid>>>();
            var entity = new TestAggregate();
            entity.Created();

            contextMock
                .Setup(context => context.GetCollection<TestAggregate, Guid>(It.IsAny<string>()))
                .Returns(collectionMock);

            contextMock
                .Setup(context => context.GetModelRepository<TestAggregate>(
                    It.IsAny<string>(),
                    It.IsAny<ILogger>()))
                .Returns(modelRepositoryMock.Object);

            modelRepositoryMock
                .Setup(modelRepository => modelRepository.InsertAsync(
                    It.IsAny<Expression<Func<TestAggregate, Guid>>>(),
                    entity,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            dispatcherMock
                .Setup(dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var repository = new WriteRepository<TestAggregate, Guid>(
                "test-collection",
                contextMock.Object,
                loggerMock.Object,
                dispatcherMock.Object);

            await repository.InsertAsync(entity, CancellationToken.None);

            Assert.Empty(entity.Events);

            dispatcherMock.Verify(
                dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Starting domain events dipatch of {0} aggregate with {1} id", typeof(TestAggregate), entity.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Dipatching {0} event", typeof(TestAggregate.CreatedEvent)))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Clearing the {0} aggregate domain events", typeof(TestAggregate)))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenDispatchingEventAndThrowException_ShouldByPassOnlyThisException()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestAggregate>>();
            var modelRepositoryMock = new Mock<IModelRepository<TestAggregate>>();
            var dispatcherMock = new Mock<IDomainEventDispatcher>();
            var loggerMock = new Mock<ILogger<WriteRepository<TestAggregate, Guid>>>();
            var entity = new TestAggregate();
            entity.Created();
            entity.Updated();

            contextMock
                .Setup(context => context.GetCollection<TestAggregate, Guid>(It.IsAny<string>()))
                .Returns(collectionMock);

            contextMock
                .Setup(context => context.GetModelRepository<TestAggregate>(
                    It.IsAny<string>(),
                    It.IsAny<ILogger>()))
                .Returns(modelRepositoryMock.Object);

            modelRepositoryMock
                .Setup(modelRepository => modelRepository.InsertAsync(
                    It.IsAny<Expression<Func<TestAggregate, Guid>>>(),
                    entity,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            dispatcherMock
                .Setup(dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<TestAggregate.CreatedEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            dispatcherMock
                .Setup(dispatcher => dispatcher.DispatchAsync(
                    It.Is<IDomainEvent>(evt => evt.GetType() == typeof(TestAggregate.UpdatedEvent)),
                    It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            var repository = new WriteRepository<TestAggregate, Guid>(
                "test-collection",
                contextMock.Object,
                loggerMock.Object,
                dispatcherMock.Object);

            await Assert.ThrowsAsync<Exception>(() => repository.InsertAsync(entity, CancellationToken.None));

            Assert.True(entity.Events.Count == 2);

            dispatcherMock.Verify(
                dispatcher => dispatcher.DispatchAsync(
                    It.Is<IDomainEvent>(evt => evt.GetType() == typeof(TestAggregate.CreatedEvent)),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            dispatcherMock.Verify(
                dispatcher => dispatcher.DispatchAsync(
                    It.Is<IDomainEvent>(evt => evt.GetType() == typeof(TestAggregate.UpdatedEvent)),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            dispatcherMock.Verify(
                dispatcher => dispatcher.DispatchAsync(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Starting domain events dipatch of {0} aggregate with {1} id", typeof(TestAggregate), entity.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Dipatching {0} event", typeof(TestAggregate.CreatedEvent)))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(1));

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Dipatching {0} event", typeof(TestAggregate.UpdatedEvent)))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(1));

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("An error was occurred on {0} event dispatch", typeof(TestAggregate.UpdatedEvent)))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(1));

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Clearing the {0} aggregate domain events", typeof(TestAggregate)))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);
        }
    }
}
