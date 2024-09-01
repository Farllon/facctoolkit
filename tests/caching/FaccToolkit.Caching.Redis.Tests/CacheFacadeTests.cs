using FaccToolkit.Caching.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;

namespace FaccToolkit.Caching.Redis.Tests
{
    public class CacheFacadeTests
    {
        private readonly Mock<IDatabase> _mockRedisDb;
        private readonly Mock<ILogger<CacheFacade>> _mockLogger;
        private readonly Mock<IModelSerializer> _mockModelSerializer;
        private readonly RedisConfiguration _redisConfig;
        private readonly CacheFacade _cacheFacade;

        public CacheFacadeTests()
        {
            _mockRedisDb = new Mock<IDatabase>();
            _mockLogger = new Mock<ILogger<CacheFacade>>();
            _mockModelSerializer = new Mock<IModelSerializer>();
            _redisConfig = new RedisConfiguration
            {
                Prefix = "test",
                ExpirationInMilliseconds = 60000,
                SuppressCacheSetErrors = true
            };

            _cacheFacade = new CacheFacade(_mockModelSerializer.Object, _mockRedisDb.Object, _mockLogger.Object, _redisConfig);
        }

        [Fact]
        public async Task TryGetAsync_KeyNotFound_ReturnsNullAndLogsWarning()
        {
            // Arrange
            var key = "non-existing-key";
            _mockRedisDb.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                        .ReturnsAsync(RedisValue.Null);

            // Act
            var result = await _cacheFacade.TryGetAsync<object>(key, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _mockLogger.VerifyLog(LogLevel.Warning, $"[Cache Miss] The key {key} was not found on cache");
        }

        [Fact]
        public async Task TryGetAsync_KeyFound_ReturnsDeserializedObjectAndLogsInformation()
        {
            // Arrange
            var key = "existing-key";
            var cachedValue = "cached value";
            var expectedObject = new { Name = "Test" };
            _mockRedisDb.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                        .ReturnsAsync(cachedValue);
            _mockModelSerializer.Setup(serializer => serializer.DeserializeAsync<object>(cachedValue, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(expectedObject);

            // Act
            var result = await _cacheFacade.TryGetAsync<object>(key, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedObject, result);
            _mockLogger.VerifyLog(LogLevel.Information, $"[Cache Hit] The key {key} was found in cache");
        }

        [Fact]
        public async Task SetAsync_SuccessfulCacheSet_LogsInformation()
        {
            // Arrange
            var key = "test-key";
            var value = new { Name = "Test" };
            var serializedValue = "serialized value";
            _mockModelSerializer.Setup(serializer => serializer.SerializeAsync(value, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(serializedValue);
            _mockRedisDb.Setup(db => db.StringSetAsync(key, serializedValue, It.IsAny<TimeSpan>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
                        .ReturnsAsync(true);

            // Act
            await _cacheFacade.SetAsync(key, value, CancellationToken.None);

            // Assert
            _mockLogger.VerifyLog(LogLevel.Information, $"Setting the value with {key} key in cache");
        }

        [Fact]
        public async Task SetAsync_FailedCacheSet_LogsWarningOrErrorBasedOnConfig()
        {
            // Arrange
            var key = "test-key";
            var value = new { Name = "Test" };
            var serializedValue = "serialized value";
            _mockModelSerializer.Setup(serializer => serializer.SerializeAsync(value, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(serializedValue);
            _mockRedisDb.Setup(db => db.StringSetAsync(key, serializedValue, It.IsAny<TimeSpan>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
                        .ReturnsAsync(false);

            // Act
            await _cacheFacade.SetAsync(key, value, CancellationToken.None);

            // Assert
            _mockLogger.VerifyLog(LogLevel.Warning, $"The cache set operation of {key} key was failed");
        }

        [Fact]
        public async Task SetAsync_FailedCacheSet_LogsError()
        {
            // Arrange
            var key = "test-key";
            var value = new { Name = "Test" };
            var serializedValue = "serialized value";
            _mockModelSerializer.Setup(serializer => serializer.SerializeAsync(value, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(serializedValue);
            _mockRedisDb.Setup(db => db.StringSetAsync(key, serializedValue, It.IsAny<TimeSpan>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
                        .ReturnsAsync(false);

            var redisConfig = new RedisConfiguration
            {
                Prefix = "test",
                ExpirationInMilliseconds = 60000,
                SuppressCacheSetErrors = false
            };

            var cacheFacade = new CacheFacade(_mockModelSerializer.Object, _mockRedisDb.Object, _mockLogger.Object, redisConfig);

            // Act
            await Assert.ThrowsAsync<SetCacheOperationException>(() => cacheFacade.SetAsync(key, value, CancellationToken.None));

            // Assert
            _mockLogger.VerifyLog(LogLevel.Error, $"The cache set operation of {key} key was failed");
        }

        [Fact]
        public async Task ExpiryAsync_KeyDeleted_LogsInformation()
        {
            // Arrange
            var key = "test-key";
            _mockRedisDb.Setup(db => db.KeyDeleteAsync(key, It.IsAny<CommandFlags>()))
                        .ReturnsAsync(true);

            // Act
            await _cacheFacade.ExpiryAsync(key, CancellationToken.None);

            // Assert
            _mockLogger.VerifyLog(LogLevel.Information, $"The {key} key was removed from the cache");
        }

        [Fact]
        public async Task ExpiryAsync_KeyNotDeleted_LogsWarning()
        {
            // Arrange
            var key = "test-key";
            _mockRedisDb.Setup(db => db.KeyDeleteAsync(key, It.IsAny<CommandFlags>()))
                        .ReturnsAsync(false);

            // Act
            await _cacheFacade.ExpiryAsync(key, CancellationToken.None);

            // Assert
            _mockLogger.VerifyLog(LogLevel.Warning, $"The {key} key was not removed from the cache");
        }

        [Fact]
        public void GenerateKey_CreatesExpectedKeyFormat()
        {
            // Arrange
            var suffix = "suffix";
            var expectedKey = $"test:{typeof(object)}:{suffix}";

            // Act
            var result = _cacheFacade.GenerateKey<object>(suffix);

            // Assert
            Assert.Equal(expectedKey, result);
        }
    }

    public static class LoggerExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel logLevel, string expectedMessage)
        {
            loggerMock.Verify(
                m => m.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}