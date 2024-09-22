using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using System.Linq.Expressions;

namespace FaccToolkit.Persistence.MongoDb.AnemicDomain.Tests
{
    public class EntityRepositoryTests
    {
        [Fact]
        public async void GivenExistingEntity_WhenFindById_ShouldReturnTheFOundEntity()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestEntity>>();
            var modelRepositoryMock = new Mock<MongoDocumentRepository<TestEntity>>();
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

            var repository = new EntityRepository<TestEntity, Guid>(
                "test-collection",
                contextMock.Object,
                Mock.Of<ILogger<EntityRepository<TestEntity, Guid>>>());

            var result = await repository.FindByIdAsync(entity.Id, CancellationToken.None);

            Assert.Equivalent(entity, result);
        }

        [Fact]
        public async Task WhenInsert_ShouldCallModelRepositoryInsert()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestEntity>>();
            var modelRepositoryMock = new Mock<MongoDocumentRepository<TestEntity>>();
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
                .Setup(modelRepository => modelRepository.InsertAsync(
                    It.IsAny<Expression<Func<TestEntity, Guid>>>(),
                    entity,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var repository = new EntityRepository<TestEntity, Guid>(
                "test-collection",
                contextMock.Object,
                Mock.Of<ILogger<EntityRepository<TestEntity, Guid>>>());

            await repository.InsertAsync(entity, CancellationToken.None);

            modelRepositoryMock.Verify(
                modelRepository => modelRepository.InsertAsync(
                    It.IsAny<Expression<Func<TestEntity, Guid>>>(),
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenInsertMany_ShouldCallModelRepositoryInsertMany()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestEntity>>();
            var modelRepositoryMock = new Mock<MongoDocumentRepository<TestEntity>>();
            var entity = new TestEntity();
            var entities = new List<TestEntity>
            {
                entity
            };

            contextMock
                .Setup(context => context.GetCollection<TestEntity, Guid>(It.IsAny<string>()))
                .Returns(collectionMock);

            contextMock
                .Setup(context => context.GetModelRepository<TestEntity>(
                    It.IsAny<string>(),
                    It.IsAny<ILogger>()))
                .Returns(modelRepositoryMock.Object);

            modelRepositoryMock
                .Setup(modelRepository => modelRepository.InsertAsync(
                    It.IsAny<Expression<Func<TestEntity, Guid>>>(),
                    entities,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var repository = new EntityRepository<TestEntity, Guid>(
                "test-collection",
                contextMock.Object,
                Mock.Of<ILogger<EntityRepository<TestEntity, Guid>>>());

            await repository.InsertAsync(entities, CancellationToken.None);

            modelRepositoryMock.Verify(
                modelRepository => modelRepository.InsertAsync(
                    It.IsAny<Expression<Func<TestEntity, Guid>>>(),
                    entities,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenUpdate_ShouldCallModelRepositoryUpdate()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestEntity>>();
            var modelRepositoryMock = new Mock<MongoDocumentRepository<TestEntity>>();
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
                .Setup(modelRepository => modelRepository.UpdateAsync(
                    It.IsAny<Expression<Func<TestEntity, Guid>>>(),
                    entity,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestEntity?>(entity));

            var repository = new EntityRepository<TestEntity, Guid>(
                "test-collection",
                contextMock.Object,
                Mock.Of<ILogger<EntityRepository<TestEntity, Guid>>>());

            await repository.UpdateAsync(entity, CancellationToken.None);

            modelRepositoryMock.Verify(
                modelRepository => modelRepository.UpdateAsync(
                    It.IsAny<Expression<Func<TestEntity, Guid>>>(),
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenDelete_ShouldCallModelRepositoryDelete()
        {
            var contextMock = new Mock<IMongoDbContext>();
            var collectionMock = Mock.Of<IMongoCollection<TestEntity>>();
            var modelRepositoryMock = new Mock<MongoDocumentRepository<TestEntity>>();
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
                .Setup(modelRepository => modelRepository.DeleteAsync(
                    It.IsAny<Expression<Func<TestEntity, Guid>>>(),
                    entity.Id,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<TestEntity?>(entity));

            var repository = new EntityRepository<TestEntity, Guid>(
                "test-collection",
                contextMock.Object,
                Mock.Of<ILogger<EntityRepository<TestEntity, Guid>>>());

            await repository.DeleteAsync(entity.Id, CancellationToken.None);

            modelRepositoryMock.Verify(
                modelRepository => modelRepository.DeleteAsync(
                    It.IsAny<Expression<Func<TestEntity, Guid>>>(),
                    entity.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
