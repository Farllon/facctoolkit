using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;

namespace FaccToolkit.Persistence.MongoDb.Abstractions.Tests
{
    public class MongoDbContextTests : IClassFixture<MongoDbFixture>
    {
        private readonly MongoDbFixture _fixture;
        private readonly TestMongoDbContext _context;
        private readonly Mock<ILogger<TestMongoDbContext>> _loggerMock;

        public MongoDbContextTests(MongoDbFixture fixture)
        {
            _fixture = fixture;
            _loggerMock = new Mock<ILogger<TestMongoDbContext>>();
            _context = new TestMongoDbContext(
                _fixture.Client!,
                _loggerMock.Object);
        }

        [Fact]
        public void WhenCreatingNewContextAndStartSessionThrowException_ShouldLogErrorAndThrow()
        {
            var clientMock = new Mock<IMongoClient>();
            var loggerMock = new Mock<ILogger<TestMongoDbContext>>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => new TestMongoDbContext(clientMock.Object, loggerMock.Object));

            loggerMock.Verify(
                logger => logger.BeginScope("Start new mongodb session flow"),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Starting the new session")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("An error was occured in the start mongodb session flow")),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void WhenCreatingNewContext_ShouldStartNewSession()
        {
            var clientMock = new Mock<IMongoClient>();
            var loggerMock = new Mock<ILogger<TestMongoDbContext>>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Mock.Of<IClientSessionHandle>());

            var context = new TestMongoDbContext(clientMock.Object, loggerMock.Object);

            Assert.NotNull(context);
            Assert.NotNull(context.CurrentSession);

            loggerMock.Verify(
                logger => logger.BeginScope("Start new mongodb session flow"),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Starting the new session")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("The new session was started")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void WhenGetCollection_ShouldReturnTheCollection()
        {
            var collection = _context.GetCollection<TestEntity, Guid>(Guid.NewGuid().ToString());

            Assert.NotNull(collection);
        }

        [Fact]
        public void WhenGetModelRepository_ShouldReturnModelRepository()
        {
            var modelRepository = _context.GetModelRepository<TestModel>(Guid.NewGuid().ToString());

            Assert.NotNull(modelRepository);
        }

        [Fact]
        public void WhenStartTransaction_ShouldCurrentSessionIsInTransaction()
        {
            _context.StartTransaction();

            Assert.True(_context.CurrentSession.IsInTransaction);

            _loggerMock.Verify(
                logger => logger.BeginScope("Start new mongodb transaction flow"),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Starting the new transaction")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("The new transaction was started")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void WhenStartTransactionThrowException_ShouldLogErrorAndThrow()
        {
            var clientMock = new Mock<IMongoClient>();
            var loggerMock = new Mock<ILogger<TestMongoDbContext>>();
            var sessionMock = new Mock<IClientSessionHandle>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(sessionMock.Object);

            sessionMock
                .Setup(session => session.StartTransaction(It.IsAny<TransactionOptions>()))
                .Throws<Exception>();

            var context = new TestMongoDbContext(clientMock.Object, loggerMock.Object);

            Assert.Throws<Exception>(() => context.StartTransaction());

            loggerMock.Verify(
                logger => logger.BeginScope("Start new mongodb transaction flow"),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Starting the new transaction")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("An error was occured in the start mongodb transaction flow")),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public void GivenNullSession_WhenStartTransacion_ShouldThrowInvalidOperationException()
        {
            var clientMock = new Mock<IMongoClient>();
            var loggerMock = new Mock<ILogger<TestMongoDbContext>>();
            var sessionMock = new Mock<IClientSessionHandle>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(null as IClientSessionHandle);

            var context = new TestMongoDbContext(clientMock.Object, loggerMock.Object);

            Assert.Throws<InvalidOperationException>(() => context.StartTransaction());

            loggerMock.Verify(
                logger => logger.BeginScope("Start new mongodb transaction flow"),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Starting the new transaction")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);
        }

        [Fact]
        public async Task WhenCommitTransaction_ShouldNotCurrentSessionIsInTransaction()
        {
            _context.StartTransaction();

            await _context.CommitTransactionAsync(CancellationToken.None);

            Assert.False(_context.CurrentSession.IsInTransaction);

            _loggerMock.Verify(
                logger => logger.BeginScope("Commit mongodb transaction flow"),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Committing the mongodb transaction")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("The transaction was committed")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenCommitTransactionThrowException_ShouldLogErrorAndThrow()
        {
            var clientMock = new Mock<IMongoClient>();
            var loggerMock = new Mock<ILogger<TestMongoDbContext>>();
            var sessionMock = new Mock<IClientSessionHandle>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(sessionMock.Object);

            sessionMock
                .Setup(session => session.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            var context = new TestMongoDbContext(clientMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<Exception>(() => context.CommitTransactionAsync(CancellationToken.None));

            loggerMock.Verify(
                logger => logger.BeginScope("Commit mongodb transaction flow"),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Committing the mongodb transaction")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("An error was occured in the commit mongodb transaction flow")),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenNullSession_WhenCommitTransacion_ShouldThrowInvalidOperationException()
        {
            var clientMock = new Mock<IMongoClient>();
            var loggerMock = new Mock<ILogger<TestMongoDbContext>>();
            var sessionMock = new Mock<IClientSessionHandle>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(null as IClientSessionHandle);

            var context = new TestMongoDbContext(clientMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => context.CommitTransactionAsync(CancellationToken.None));

            loggerMock.Verify(
                logger => logger.BeginScope("Commit mongodb transaction flow"),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Committing the mongodb transaction")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);
        }

        [Fact]
        public async Task WhenAbortTransaction_ShouldNotCurrentSessionIsInTransaction()
        {
            _context.StartTransaction();

            await _context.AbortTransactionAsync(CancellationToken.None);

            Assert.False(_context.CurrentSession.IsInTransaction);

            _loggerMock.Verify(
                logger => logger.BeginScope("Abort mongodb transaction flow"),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Aborting the mongodb transaction")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("The transaction was aborted")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenAbortTransactionThrowException_ShouldLogErrorAndThrow()
        {
            var clientMock = new Mock<IMongoClient>();
            var loggerMock = new Mock<ILogger<TestMongoDbContext>>();
            var sessionMock = new Mock<IClientSessionHandle>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(sessionMock.Object);

            sessionMock
                .Setup(session => session.AbortTransactionAsync(It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            var context = new TestMongoDbContext(clientMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<Exception>(() => context.AbortTransactionAsync(CancellationToken.None));

            loggerMock.Verify(
                logger => logger.BeginScope("Abort mongodb transaction flow"),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Aborting the mongodb transaction")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("An error was occured in the abort mongodb transaction flow")),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenNullSession_WhenAbortTransacion_ShouldThrowInvalidOperationException()
        {
            var clientMock = new Mock<IMongoClient>();
            var loggerMock = new Mock<ILogger<TestMongoDbContext>>();
            var sessionMock = new Mock<IClientSessionHandle>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(null as IClientSessionHandle);

            var context = new TestMongoDbContext(clientMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => context.AbortTransactionAsync(CancellationToken.None));

            loggerMock.Verify(
                logger => logger.BeginScope("Abort mongodb transaction flow"),
                Times.Once);

            loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Aborting the mongodb transaction")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never);
        }
    }
}
