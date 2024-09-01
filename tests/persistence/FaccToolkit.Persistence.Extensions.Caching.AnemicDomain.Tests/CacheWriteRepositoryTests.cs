using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Domain.Anemic;
using Microsoft.Extensions.Logging;
using Moq;

namespace FaccToolkit.Persistence.Extensions.Caching.AnemicDomain.Tests
{
    public class CacheWriteRepositoryTests
    {
        [Fact]
        public async Task WhenInsert_ShouldSetOnCache()
        {
            var cacheFacadeMock = new Mock<ICacheFacade>();
            var repositoryMock = new Mock<IWriteRepository<TestEntity, Guid>>();
            var loggerMock = new Mock<ILogger<CacheWriteRepository<IWriteRepository<TestEntity, Guid>, TestEntity, Guid>>>();
            var entity = new TestEntity();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestEntity>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            var cacheRepository = new CacheWriteRepository<IWriteRepository<TestEntity, Guid>, TestEntity, Guid>(
                cacheFacadeMock.Object,
                repositoryMock.Object,
                loggerMock.Object);

            await cacheRepository.InsertAsync(entity, CancellationToken.None);

            cacheFacadeMock.Verify(
                facade => facade.SetAsync(
                    entity.Id.ToString(),
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            // Todo: verify logs
        }

        [Fact]
        public async Task WhenInsertMany_ShouldSetEachOnCache()
        {
            var cacheFacadeMock = new Mock<ICacheFacade>();
            var repositoryMock = new Mock<IWriteRepository<TestEntity, Guid>>();
            var loggerMock = new Mock<ILogger<CacheWriteRepository<IWriteRepository<TestEntity, Guid>, TestEntity, Guid>>>();
            var entity = new TestEntity();
            var entities = new List<TestEntity> { entity };

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestEntity>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            var cacheRepository = new CacheWriteRepository<IWriteRepository<TestEntity, Guid>, TestEntity, Guid>(
                cacheFacadeMock.Object,
                repositoryMock.Object,
                loggerMock.Object);

            await cacheRepository.InsertAsync(entities, CancellationToken.None);

            cacheFacadeMock.Verify(
                facade => facade.SetAsync(
                    entity.Id.ToString(),
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Exactly(entities.Count));

            // Todo: verify logs
        }

        [Fact]
        public async Task WhenUpdate_ShouldSetOnCache()
        {
            var cacheFacadeMock = new Mock<ICacheFacade>();
            var repositoryMock = new Mock<IWriteRepository<TestEntity, Guid>>();
            var loggerMock = new Mock<ILogger<CacheWriteRepository<IWriteRepository<TestEntity, Guid>, TestEntity, Guid>>>();
            var entity = new TestEntity();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestEntity>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            var cacheRepository = new CacheWriteRepository<IWriteRepository<TestEntity, Guid>, TestEntity, Guid>(
                cacheFacadeMock.Object,
                repositoryMock.Object,
                loggerMock.Object);

            await cacheRepository.UpdateAsync(entity, CancellationToken.None);

            cacheFacadeMock.Verify(
                facade => facade.SetAsync(
                    entity.Id.ToString(),
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            // Todo: verify logs
        }

        [Fact]
        public async Task WhenDelete_ShouldExpireFromCache()
        {
            var cacheFacadeMock = new Mock<ICacheFacade>();
            var repositoryMock = new Mock<IWriteRepository<TestEntity, Guid>>();
            var loggerMock = new Mock<ILogger<CacheWriteRepository<IWriteRepository<TestEntity, Guid>, TestEntity, Guid>>>();
            var entity = new TestEntity();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestEntity>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            var cacheRepository = new CacheWriteRepository<IWriteRepository<TestEntity, Guid>, TestEntity, Guid>(
                cacheFacadeMock.Object,
                repositoryMock.Object,
                loggerMock.Object);

            await cacheRepository.DeleteAsync(entity.Id, CancellationToken.None);

            cacheFacadeMock.Verify(
                facade => facade.ExpiryAsync(
                    entity.Id.ToString(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            // Todo: verify logs
        }
    }
}
