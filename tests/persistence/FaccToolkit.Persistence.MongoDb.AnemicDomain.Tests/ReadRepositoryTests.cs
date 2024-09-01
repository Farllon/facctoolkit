using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using System.Linq.Expressions;

namespace FaccToolkit.Persistence.MongoDb.AnemicDomain.Tests
{
    public partial class ReadRepositoryTests
    {

        [Fact]
        public async void GivenExistingEntity_WhenFindById_ShouldReturnTheFOundEntity()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestEntity>>();
            var modelRepositoryMock = new Mock<IModelRepository<TestEntity>>();
            var entity = new TestEntity();

            contextMock
                .Setup(context => context.GetCollection<TestEntity, Guid>(It.IsAny<string>()))
                .Returns(collectionMock);

            contextMock
                .Setup(context => context.GetModelRepository<TestEntity>(
                    It.IsAny<string>(),
                    It.IsAny<ILogger>()))
                .Returns(modelRepositoryMock.Object);

            modelRepositoryMock
                .Setup(modelRepository => modelRepository.FindByIdAsync<Guid>(
                    It.IsAny<Expression<Func<TestEntity, Guid>>>(),
                    entity.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestEntity?>(entity));

            var repository = new ReadRepository<TestEntity, Guid>(
                "test-collection",
                contextMock.Object,
                Mock.Of<ILogger<ReadRepository<TestEntity, Guid>>>());

            var result = await repository.FindByIdAsync(entity.Id, CancellationToken.None);

            Assert.Equivalent(entity, result);
        }
    }
}