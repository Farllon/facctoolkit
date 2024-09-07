using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data.Common;
using System.Diagnostics;

namespace FaccToolkit.Persistence.EntityFramework.Abstractions.Tests
{
    public partial class BaseRepositoryTests : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly TestContext _context;
        private readonly Mock<ILogger<BaseRepository<TestModel, TestContext>>> _loggerMock;
        private readonly BaseRepository<TestModel, TestContext> _baseRepository;

        public BaseRepositoryTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var contextOptions = new DbContextOptionsBuilder<TestContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .LogTo(log => Debug.WriteLine(log))
                .Options;

            _context = new TestContext(contextOptions);
            
            _context.Database.EnsureCreated();

            _loggerMock = new Mock<ILogger<BaseRepository<TestModel, TestContext>>>();
            _baseRepository = new BaseRepository<TestModel, TestContext>(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task WhenInsert_ShouldSuccess()
        {
            var model = new TestModel();

            await _baseRepository.InsertAsync(model => model.Id, model, CancellationToken.None);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting {0} model with {1} id in database", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Successfully inserted {0} model with {1} id", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenAnExistingDocumentInDatabase_WhenInsertTheSameDocument_ShouldReturnError()
        {
            var existing = new TestModel();
            var model = new TestModel()
            {
                Id = existing.Id
            };

            await _baseRepository.InsertAsync(m => m.Id, existing, CancellationToken.None);

            await Assert.ThrowsAnyAsync<Exception>(() => _baseRepository.InsertAsync(model => model.Id, model, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting {0} model with {1} id in database", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Failed to insert {0} model with {1} id", typeof(TestModel), model.Id))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task WhenInsertAndReceiveException_ShouldThrowException()
        {
            var model = new TestModel();

            var contextMock = new Mock<TestContext>();
            var dbsetMock = new Mock<DbSet<TestModel>>();

            contextMock
                .Setup(context => context.Set<TestModel>())
                .Returns(dbsetMock.Object);

            dbsetMock
                .Setup(dbset => dbset.AddAsync(
                    model,
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromException<EntityEntry<TestModel>>(new Exception()));

            var repository = new BaseRepository<TestModel, TestContext>(
                contextMock.Object,
                _loggerMock.Object);

            await Record.ExceptionAsync(() => repository.InsertAsync(model => model.Id, model, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting {0} model with {1} id in database", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Failed to insert {0} model with {1} id", typeof(TestModel), model.Id))),
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

            await _baseRepository.InsertAsync(models, CancellationToken.None);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting multiple {0} models into the database", typeof(TestModel)))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Successfully inserted multiple {0} models", typeof(TestModel)))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task GivenAnExistingDocumentInDatabase_WhenInsertMany_ShouldThrowException()
        {
            var existing = new TestModel();
            var model = new TestModel()
            {
                Id = existing.Id
            };
            var models = new List<TestModel> { model };

            await _baseRepository.InsertAsync(m => m.Id, existing, CancellationToken.None);

            await Assert.ThrowsAnyAsync<Exception>(() => _baseRepository.InsertAsync(models, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting multiple {0} models into the database", typeof(TestModel)))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Failed to insert multiple {0} models", typeof(TestModel)))),
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

            var contextMock = new Mock<TestContext>();
            var dbsetMock = new Mock<DbSet<TestModel>>();

            contextMock
                .Setup(context => context.Set<TestModel>())
                .Returns(dbsetMock.Object);

            dbsetMock
                .Setup(dbset => dbset.AddRangeAsync(
                    models,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromException(new Exception()));

            var repository = new BaseRepository<TestModel, TestContext>(
                contextMock.Object,
                _loggerMock.Object);

            await Record.ExceptionAsync(() => repository.InsertAsync(models, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Inserting multiple {0} models into the database", typeof(TestModel)))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Failed to insert multiple {0} models", typeof(TestModel)))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenAnExistingDocument_WhenUpdate_ShouldReturnTheOldDocument()
        {
            var model = new TestModel();

            await _baseRepository.InsertAsync(model => model.Id, model, CancellationToken.None);

            var oldProperty = model.MyProperty;

            model.MyProperty = Guid.NewGuid().ToString();

            await _baseRepository.UpdateAsync(model => model.Id, model, CancellationToken.None);
            
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Updating {0} model with {1} id in database", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Successfully updated {0} model with {1} id", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task GivenAnNonExistingDocument_WhenUpdate_ShouldReturnNull()
        {
            var model = new TestModel();

            await _baseRepository.UpdateAsync(model => model.Id, model, CancellationToken.None);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Updating {0} model with {1} id in database", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id was not found", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task WhenUpdateAndReceiveException_ShouldThrowException()
        {
            var model = new TestModel();

            var contextMock = new Mock<TestContext>();
            var dbsetMock = new Mock<DbSet<TestModel>>();

            contextMock
                .Setup(context => context.Set<TestModel>())
                .Returns(dbsetMock.Object);

            dbsetMock
                .Setup(dbset => dbset.Update(model))
                .Throws<Exception>();

            dbsetMock
                .Setup(dbset => dbset.FindAsync(
                    It.IsAny<object?[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult<TestModel?>(model));

            var repository = new BaseRepository<TestModel, TestContext>(
                contextMock.Object,
                _loggerMock.Object);

            await Record.ExceptionAsync(() => repository.UpdateAsync(model => model.Id, model, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Updating {0} model with {1} id in database", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Failed to update {0} model with {1} id", typeof(TestModel), model.Id))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GivenAnExistingDocument_WhenDelete_ShouldReturnTheOldDocument()
        {
            var model = new TestModel();

            await _baseRepository.InsertAsync(model => model.Id, model, CancellationToken.None);

            await _baseRepository.DeleteAsync(model.Id, CancellationToken.None);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Deleting {0} model with {1} id from the database", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Successfully deleted {0} model with {1} id from the database", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task GivenAnNonExistingDocument_WhenDelete_ShouldReturnNull()
        {
            var model = new TestModel();

            await _baseRepository.DeleteAsync(model.Id, CancellationToken.None);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Deleting {0} model with {1} id from the database", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("The {0} model with {1} id was not found", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task WhenDeleteAndReceiveException_ShouldThrowException()
        {
            var model = new TestModel();

            var contextMock = new Mock<TestContext>();
            var dbsetMock = new Mock<DbSet<TestModel>>();

            contextMock
                .Setup(context => context.Set<TestModel>())
                .Returns(dbsetMock.Object);

            dbsetMock
                .Setup(dbset => dbset.Remove(model))
                .Throws<Exception>();

            dbsetMock
                .Setup(dbset => dbset.FindAsync(
                    It.IsAny<object?[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult<TestModel?>(model));

            var repository = new BaseRepository<TestModel, TestContext>(
                contextMock.Object,
                _loggerMock.Object);

            await Record.ExceptionAsync(() => repository.DeleteAsync(model.Id, CancellationToken.None));

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Deleting {0} model with {1} id from the database", typeof(TestModel), model.Id))),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(string.Format("Failed to delete {0} model with {1} id from the database", typeof(TestModel), model.Id))),
                    It.IsNotNull<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        public void Dispose()
        {
            _baseRepository.DisposeAsync().GetAwaiter().GetResult();
            _context.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
