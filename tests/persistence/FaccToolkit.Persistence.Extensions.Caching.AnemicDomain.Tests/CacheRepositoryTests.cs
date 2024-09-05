using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Domain.Anemic;
using Microsoft.Extensions.Logging;
using Moq;

namespace FaccToolkit.Persistence.Extensions.Caching.AnemicDomain.Tests
{
    public class CacheRepositoryTests
    {
        [Fact]
        public async Task GivenAnEntityExistingInCache_WhenFind_ShouldReturnCacheEntity()
        {
            var cacheFacadeMock = new Mock<ICacheFacade>();
            var repositoryMock = new Mock<IEntityRepository<TestEntity, Guid>>();
            var loggerMock = new Mock<ILogger<CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>>>();
            var entity = new TestEntity();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestEntity>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            cacheFacadeMock
                .Setup(facade => facade.TryGetAsync<TestEntity>(
                    entity.Id.ToString(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestEntity?>(entity));

            var cacheRepository = new CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>(
                cacheFacadeMock.Object,
                repositoryMock.Object,
                loggerMock.Object);

            var found = await cacheRepository.FindByIdAsync(entity.Id, CancellationToken.None);

            Assert.NotNull(found);

            // Todo: verify logs
        }

        [Fact]
        public async Task GivenAnEntityNonExistingInCache_WhenFind_ShouldCallRepositoryAndSaveInCache()
        {
            var cacheFacadeMock = new Mock<ICacheFacade>();
            var repositoryMock = new Mock<IEntityRepository<TestEntity, Guid>>();
            var loggerMock = new Mock<ILogger<CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>>>();
            var entity = new TestEntity();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestEntity>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            cacheFacadeMock
                .Setup(facade => facade.TryGetAsync<TestEntity>(
                    entity.Id.ToString(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestEntity?>(null));

            repositoryMock
                .Setup(repository => repository.FindByIdAsync(
                    entity.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestEntity?>(entity));

            var cacheRepository = new CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>(
                cacheFacadeMock.Object,
                repositoryMock.Object,
                loggerMock.Object);

            var found = await cacheRepository.FindByIdAsync(entity.Id, CancellationToken.None);

            Assert.NotNull(found);

            cacheFacadeMock.Verify(
                facade => facade.SetAsync(
                    entity.Id.ToString(),
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            // Todo: verify logs
        }

        [Fact]
        public async Task GivenAnEntityNonExistingInCacheAndDatabase_WhenFind_ShouldCallRepositoryAndNotSaveInCache()
        {
            var cacheFacadeMock = new Mock<ICacheFacade>();
            var repositoryMock = new Mock<IEntityRepository<TestEntity, Guid>>();
            var loggerMock = new Mock<ILogger<CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>>>();
            var entity = new TestEntity();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestEntity>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            cacheFacadeMock
                .Setup(facade => facade.TryGetAsync<TestEntity>(
                    entity.Id.ToString(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestEntity?>(null));

            repositoryMock
                .Setup(repository => repository.FindByIdAsync(
                    entity.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestEntity?>(null));

            var cacheRepository = new CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>(
                cacheFacadeMock.Object,
                repositoryMock.Object,
                loggerMock.Object);

            var found = await cacheRepository.FindByIdAsync(entity.Id, CancellationToken.None);

            Assert.Null(found);

            cacheFacadeMock.Verify(
                facade => facade.SetAsync(
                    entity.Id.ToString(),
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Never);

            // Todo: verify logs
        }

        [Fact]
        public async Task WhenInsert_ShouldSetOnCache()
        {
            var cacheFacadeMock = new Mock<ICacheFacade>();
            var repositoryMock = new Mock<IEntityRepository<TestEntity, Guid>>();
            var loggerMock = new Mock<ILogger<CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>>>();
            var entity = new TestEntity();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestEntity>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            var cacheRepository = new CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>(
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
            var repositoryMock = new Mock<IEntityRepository<TestEntity, Guid>>();
            var loggerMock = new Mock<ILogger<CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>>>();
            var entity = new TestEntity();
            var entities = new List<TestEntity> { entity };

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestEntity>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            var cacheRepository = new CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>(
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
            var repositoryMock = new Mock<IEntityRepository<TestEntity, Guid>>();
            var loggerMock = new Mock<ILogger<CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>>>();
            var entity = new TestEntity();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestEntity>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            var cacheRepository = new CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>(
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
            var repositoryMock = new Mock<IEntityRepository<TestEntity, Guid>>();
            var loggerMock = new Mock<ILogger<CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>>>();
            var entity = new TestEntity();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestEntity>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            var cacheRepository = new CacheRepository<IEntityRepository<TestEntity, Guid>, TestEntity, Guid>(
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
