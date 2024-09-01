using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Xunit.Abstractions;

namespace FaccToolkit.Persistence.MongoDb.Abstractions.Tests
{
    public partial class ModelRepositoryTests : IClassFixture<MongoDbFixture>, IDisposable
    {
        private readonly MongoDbFixture _fixture;
        private readonly ITestOutputHelper _output;
        private readonly TestMongoDbContext _context;
        private readonly Mock<ILogger<TestMongoDbContext>> _loggerMock;
        private readonly IModelRepository<TestModel> _modelRepository;
        private const string _collectionName = "test-collection";

        public ModelRepositoryTests(MongoDbFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _loggerMock = new Mock<ILogger<TestMongoDbContext>>();
            _context = new TestMongoDbContext(
                _fixture.Client!,
                _loggerMock.Object);
            _modelRepository = _context.GetModelRepository<TestModel>(_collectionName);
            _output = output;
        }

        public void Dispose()
        {
            _output.WriteLine("Used container id: {0}", _fixture.ContainerId);
            _context?.Dispose();
        }

        [Fact]
        public async Task GivenAnExistingModelInCollection_WhenFindByById_ShouldReturnTheFoundDocument()
        {
            var existingModel = new TestModel();

            await _modelRepository.InsertAsync(model => model.Id, existingModel, CancellationToken.None);

            var foundModel = await _modelRepository.FindByIdAsync(model => model.Id, existingModel.Id, CancellationToken.None);

            Assert.Equivalent(existingModel, foundModel);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Finding {0} model with {1} id in mongo {2} collection", typeof(TestModel), existingModel.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id was found in mongo {2} collection", typeof(TestModel), existingModel.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenAnNonExistingModelInCollection_WhenFindByById_ShouldReturnNull()
        {
            var id = Guid.NewGuid();
            var foundModel = await _modelRepository.FindByIdAsync(model => model.Id, id, CancellationToken.None);

            Assert.Null(foundModel);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Finding {0} model with {1} id in mongo {2} collection", typeof(TestModel), id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id was not found in mongo {2} collection", typeof(TestModel), id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenFindByByIdAndReceiveException_ShouldReturnNull()
        {
            var id = Guid.NewGuid();
            var collectionMock = new Mock<IMongoCollection<TestModel>>();

            collectionMock.SetupGet(collection => collection.CollectionNamespace).Returns(new CollectionNamespace("a", _collectionName));

            var repository = new ModelRepository<TestModel>(
                _loggerMock.Object,
                _context,
                collectionMock.Object);

            await Assert.ThrowsAnyAsync<Exception>(() => repository.FindByIdAsync(model => model.Id, id, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Finding {0} model with {1} id in mongo {2} collection", typeof(TestModel), id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id find operation in mongo {2} collection was failed", typeof(TestModel), id, _collectionName))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenInsert_ShouldSuccess()
        {
            var model = new TestModel();

            await _modelRepository.InsertAsync(model => model.Id, model, CancellationToken.None);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting one {0} model with {1} id in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id insertion in mongo {2} collection was completed", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenAnExistingDocumentInCollection_WhenInsertTheSameDocument_ShouldReturnError()
        {
            var model = new TestModel();

            await _modelRepository.InsertAsync(model => model.Id, model, CancellationToken.None);

            await Assert.ThrowsAnyAsync<Exception>(() => _modelRepository.InsertAsync(model => model.Id, model, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting one {0} model with {1} id in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id insertion in mongo {2} collection was failed", typeof(TestModel), model.Id, _collectionName))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenInsertAndReceiveException_ShouldThrowException()
        {
            var model = new TestModel();

            var collectionMock = new Mock<IMongoCollection<TestModel>>();

            collectionMock.SetupGet(collection => collection.CollectionNamespace).Returns(new CollectionNamespace("a", _collectionName));

            collectionMock
                .Setup(collection => collection.InsertOneAsync(
                    It.IsAny<IClientSessionHandle>(),
                    model,
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromException(new Exception()));

            var repository = new ModelRepository<TestModel>(
                _loggerMock.Object,
                _context,
                collectionMock.Object);

            await Record.ExceptionAsync(() => repository.InsertAsync(model => model.Id, model, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting one {0} model with {1} id in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id insertion in mongo {2} collection was failed", typeof(TestModel), model.Id, _collectionName))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenInsertMany_ShouldSuccess()
        {
            var models = new List<TestModel>
            {
                new TestModel()
            };

            await _modelRepository.InsertAsync(model => model.Id, models, CancellationToken.None);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting many {0} models in mongo {1} collection", typeof(TestModel), _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} models insertion in mongo {1} collection was completed", typeof(TestModel), _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task GivenAnExistingDocumentInCollection_WhenInsertMany_ShouldThrowException()
        {
            var model = new TestModel();
            var models = new List<TestModel> { model };

            await _modelRepository.InsertAsync(model => model.Id, model, CancellationToken.None);

            await Assert.ThrowsAnyAsync<Exception>(() => _modelRepository.InsertAsync(model => model.Id, models, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting many {0} models in mongo {1} collection", typeof(TestModel), _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} models insertion in mongo {1} collection was failed", typeof(TestModel), _collectionName))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenInsertManyAndReceiveException_ShouldThrowException()
        {
            var models = new List<TestModel>
            {
                new TestModel()
            };

            var collectionMock = new Mock<IMongoCollection<TestModel>>();

            collectionMock.SetupGet(collection => collection.CollectionNamespace).Returns(new CollectionNamespace("a", _collectionName));

            collectionMock
                .Setup(collection => collection.InsertManyAsync(
                    It.IsAny<IClientSessionHandle>(),
                    models,
                    It.IsAny<InsertManyOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromException(new Exception()));

            var repository = new ModelRepository<TestModel>(
                _loggerMock.Object,
                _context,
                collectionMock.Object);

            await Record.ExceptionAsync(() => repository.InsertAsync(model => model.Id, models, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting many {0} models in mongo {1} collection", typeof(TestModel), _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} models insertion in mongo {1} collection was failed", typeof(TestModel), _collectionName))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenAnExistingDocument_WhenUpdate_ShouldReturnTheOldDocument()
        {
            var model = new TestModel();

            await _modelRepository.InsertAsync(model => model.Id, model, CancellationToken.None);

            var oldProperty = model.MyProperty;

            model.MyProperty = Guid.NewGuid().ToString();

            var updated = await _modelRepository.UpdateAsync(model => model.Id, model, CancellationToken.None);

            Assert.NotNull(updated);
            Assert.Equal(oldProperty, updated.MyProperty);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Updating the {0} model with {1} id in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id was was updated in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task GivenAnNonExistingDocument_WhenUpdate_ShouldReturnNull()
        {
            var model = new TestModel();

            var updated = await _modelRepository.UpdateAsync(model => model.Id, model, CancellationToken.None);

            Assert.Null(updated);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Updating the {0} model with {1} id in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id was not found in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task WhenUpdateAndReceiveException_ShouldThrowException()
        {
            var model = new TestModel();

            var collectionMock = new Mock<IMongoCollection<TestModel>>();

            collectionMock.SetupGet(collection => collection.CollectionNamespace).Returns(new CollectionNamespace("a", _collectionName));

            collectionMock
                .Setup(collection => collection.FindOneAndReplaceAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<FilterDefinition<TestModel>>(),
                    model,
                    It.IsAny<FindOneAndReplaceOptions<TestModel>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromException<TestModel>(new Exception()));

            var repository = new ModelRepository<TestModel>(
                _loggerMock.Object,
                _context,
                collectionMock.Object);

            await Record.ExceptionAsync(() => repository.UpdateAsync(model => model.Id, model, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Updating the {0} model with {1} id in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id update operation in mongo {2} collection was failed", typeof(TestModel), model.Id, _collectionName))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenAnExistingDocument_WhenDelete_ShouldReturnTheOldDocument()
        {
            var model = new TestModel();

            await _modelRepository.InsertAsync(model => model.Id, model, CancellationToken.None);

            var deleted = await _modelRepository.DeleteAsync(model => model.Id, model.Id, CancellationToken.None);

            Assert.NotNull(deleted);
            Assert.Equivalent(model, deleted);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Deleting the {0} model with {1} id in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id was was deleted in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task GivenAnNonExistingDocument_WhenDelete_ShouldReturnNull()
        {
            var model = new TestModel();

            var deleted = await _modelRepository.DeleteAsync(model => model.Id, model.Id, CancellationToken.None);

            Assert.Null(deleted);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Deleting the {0} model with {1} id in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id was not found in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task WhenDeleteAndReceiveException_ShouldThrowException()
        {
            var model = new TestModel();

            var collectionMock = new Mock<IMongoCollection<TestModel>>();

            collectionMock.SetupGet(collection => collection.CollectionNamespace).Returns(new CollectionNamespace("a", _collectionName));

            collectionMock
                .Setup(collection => collection.FindOneAndDeleteAsync(
                    It.IsAny<IClientSessionHandle>(),
                    It.IsAny<FilterDefinition<TestModel>>(),
                    It.IsAny<FindOneAndDeleteOptions<TestModel>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromException<TestModel>(new Exception()));

            var repository = new ModelRepository<TestModel>(
                _loggerMock.Object,
                _context,
                collectionMock.Object);

            await Record.ExceptionAsync(() => repository.DeleteAsync(model => model.Id, model.Id, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Deleting the {0} model with {1} id in mongo {2} collection", typeof(TestModel), model.Id, _collectionName))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id delete operation in mongo {2} collection was failed", typeof(TestModel), model.Id, _collectionName))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
