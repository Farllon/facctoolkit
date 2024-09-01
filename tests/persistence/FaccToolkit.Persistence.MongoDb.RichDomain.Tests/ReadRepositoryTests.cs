using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using System.Linq.Expressions;

namespace FaccToolkit.Persistence.MongoDb.RichDomain.Tests
{
    public partial class ReadRepositoryTests
    {

        [Fact]
        public async void GivenExistingEntity_WhenFindById_ShouldReturnTheFOundEntity()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestAggregate>>();
            var modelRepositoryMock = new Mock<IModelRepository<TestAggregate>>();
            var entity = new TestAggregate();

            contextMock
                .Setup(context => context.GetCollection<TestAggregate, Guid>(It.IsAny<string>()))
                .Returns(collectionMock);

            contextMock
                .Setup(context => context.GetModelRepository<TestAggregate>(
                    It.IsAny<string>(),
                    It.IsAny<ILogger>()))
                .Returns(modelRepositoryMock.Object);

            modelRepositoryMock
                .Setup(modelRepository => modelRepository.FindByIdAsync<Guid>(
                    It.IsAny<Expression<Func<TestAggregate, Guid>>>(),
                    entity.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestAggregate?>(entity));

            var repository = new ReadRepository<TestAggregate, Guid>(
                "test-collection",
                contextMock.Object,
                Mock.Of<ILogger<ReadRepository<TestAggregate, Guid>>>());

            var result = await repository.FindByIdAsync(entity.Id, CancellationToken.None);

            Assert.Equivalent(entity, result);
        }
    }
}