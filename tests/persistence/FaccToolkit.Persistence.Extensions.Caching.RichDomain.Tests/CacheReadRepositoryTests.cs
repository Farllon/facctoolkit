using FaccToolkit.Caching.Abstractions;
using FaccToolkit.Domain.Rich;
using Microsoft.Extensions.Logging;
using Moq;

namespace FaccToolkit.Persistence.Extensions.Caching.RichDomain.Tests
{
    public partial class CacheReadRepositoryTests
    {
        [Fact]
        public async Task GivenAnEntityExistingInCache_WhenFind_ShouldReturnCacheEntity()
        {
            var cacheFacadeMock = new Mock<ICacheFacade>();
            var repositoryMock = new Mock<IReadRepository<TestAggregate, Guid>>();
            var loggerMock = new Mock<ILogger<CacheReadRepository<IReadRepository<TestAggregate, Guid>, TestAggregate, Guid>>>();
            var entity = new TestAggregate();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestAggregate>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            cacheFacadeMock
                .Setup(facade => facade.TryGetAsync<TestAggregate>(
                    entity.Id.ToString(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestAggregate?>(entity));

            var cacheRepository = new CacheReadRepository<IReadRepository<TestAggregate, Guid>, TestAggregate, Guid>(
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
            var repositoryMock = new Mock<IReadRepository<TestAggregate, Guid>>();
            var loggerMock = new Mock<ILogger<CacheReadRepository<IReadRepository<TestAggregate, Guid>, TestAggregate, Guid>>>();
            var entity = new TestAggregate();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestAggregate>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            cacheFacadeMock
                .Setup(facade => facade.TryGetAsync<TestAggregate>(
                    entity.Id.ToString(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestAggregate?>(null));

            repositoryMock
                .Setup(repository => repository.FindByIdAsync(
                    entity.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestAggregate?>(entity));

            var cacheRepository = new CacheReadRepository<IReadRepository<TestAggregate, Guid>, TestAggregate, Guid>(
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
            var repositoryMock = new Mock<IReadRepository<TestAggregate, Guid>>();
            var loggerMock = new Mock<ILogger<CacheReadRepository<IReadRepository<TestAggregate, Guid>, TestAggregate, Guid>>>();
            var entity = new TestAggregate();

            cacheFacadeMock
                .Setup(facade => facade.GenerateKey<TestAggregate>(entity.Id.ToString()))
                .Returns(entity.Id.ToString());

            cacheFacadeMock
                .Setup(facade => facade.TryGetAsync<TestAggregate>(
                    entity.Id.ToString(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestAggregate?>(null));

            repositoryMock
                .Setup(repository => repository.FindByIdAsync(
                    entity.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestAggregate?>(null));

            var cacheRepository = new CacheReadRepository<IReadRepository<TestAggregate, Guid>, TestAggregate, Guid>(
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
    }
}
