using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;

namespace FaccToolkit.Persistence.MongoDb.Abstractions.Tests
{
    public class MongoDbUnitOfWorkTests
    {
        [Fact]
        public async Task GivenStartedAndInTransactionSession_WhenCallUowIsInTransaction_ShouldReturnTrue()
        {
            var providerMock = Mock.Of<IServiceProvider>();
            var clientMock = new Mock<IMongoClient>();
            var sessionMock = new Mock<IClientSessionHandle>();
            var expectedIsInTransaction = true;

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(sessionMock.Object);

            sessionMock
                .SetupGet(session => session.IsInTransaction)
                .Returns(expectedIsInTransaction);

            var contextMock = new TestMongoDbContext(
                clientMock.Object,
                Mock.Of<ILogger<TestMongoDbContext>>());

            var uow = new MongoDbUnitOfWork<TestMongoDbContext>(providerMock, contextMock);

            var isInTranscion = await uow.IsInTransactionAsync(CancellationToken.None);

            Assert.Equal(expectedIsInTransaction, isInTranscion);
        }

        [Fact]
        public async Task GivenStartedSession_WhenCallUowBeginTransaction_ShouldStartANewTransactionInSession()
        {
            var providerMock = Mock.Of<IServiceProvider>();
            var clientMock = new Mock<IMongoClient>();
            var sessionMock = new Mock<IClientSessionHandle>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(sessionMock.Object);

            var contextMock = new TestMongoDbContext(
                clientMock.Object,
                Mock.Of<ILogger<TestMongoDbContext>>());

            var uow = new MongoDbUnitOfWork<TestMongoDbContext>(providerMock, contextMock);

            await uow.BeginTransactionAsync(CancellationToken.None);

            sessionMock.Verify(
                session => session.StartTransaction(It.IsAny<TransactionOptions>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenStartedSession_WhenCallUowCommitTransaction_ShouldCommitTransaction()
        {
            var providerMock = Mock.Of<IServiceProvider>();
            var clientMock = new Mock<IMongoClient>();
            var sessionMock = new Mock<IClientSessionHandle>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(sessionMock.Object);

            var contextMock = new TestMongoDbContext(
                clientMock.Object,
                Mock.Of<ILogger<TestMongoDbContext>>());

            var uow = new MongoDbUnitOfWork<TestMongoDbContext>(providerMock, contextMock);

            await uow.CommitTransactionAsync(CancellationToken.None);

            sessionMock.Verify(
                session => session.CommitTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenStartedSession_WhenCallUowAbortTransaction_ShouldAbortTransaction()
        {
            var providerMock = Mock.Of<IServiceProvider>();
            var clientMock = new Mock<IMongoClient>();
            var sessionMock = new Mock<IClientSessionHandle>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(sessionMock.Object);

            var contextMock = new TestMongoDbContext(
                clientMock.Object,
                Mock.Of<ILogger<TestMongoDbContext>>());

            var uow = new MongoDbUnitOfWork<TestMongoDbContext>(providerMock, contextMock);

            await uow.AbortTransactionAsync(CancellationToken.None);

            sessionMock.Verify(
                session => session.AbortTransactionAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void WhenDisposeTheUow_ShouldDisposeContext()
        {
            var providerMock = Mock.Of<IServiceProvider>();
            var clientMock = new Mock<IMongoClient>();
            var sessionMock = new Mock<IClientSessionHandle>();

            clientMock
                .Setup(client => client.StartSession(
                    It.IsAny<ClientSessionOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(sessionMock.Object);

            var contextMock = new TestMongoDbContext(
                clientMock.Object,
                Mock.Of<ILogger<TestMongoDbContext>>());

            var uow = new MongoDbUnitOfWork<TestMongoDbContext>(providerMock, contextMock);

            uow.Dispose();

            sessionMock.Verify(session => session.Dispose(), Times.Once);
        }
    }
}