using FaccToolkit.Caching.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Caching.Redis
{
    public class CacheFacade : ICacheFacade
    {
        protected readonly string _prefix;
        protected readonly IDatabase _redisDb;
        protected readonly int _expirationInMilliseconds;
        protected readonly bool _suppressCacheSetErrors;
        protected readonly ILogger _logger;
        protected readonly IModelSerializer _modelSerializer;

        public CacheFacade(IModelSerializer modelSerializer, IDatabase redisDb, ILogger logger, RedisConfiguration cacheConfig)
        {
            _logger = logger;
            _prefix = cacheConfig.Prefix;
            _redisDb = redisDb;
            _modelSerializer = modelSerializer;
            _expirationInMilliseconds = cacheConfig.ExpirationInMilliseconds;
            _suppressCacheSetErrors = cacheConfig.SuppressCacheSetErrors;
        }

        public virtual async Task<TModel?> TryGetAsync<TModel>(string key, CancellationToken cancellationToken)
            where TModel : class
        {
            using var scope = _logger.BeginScope("Trying to find the {Key} key on the cache", key);

            var redisValue = await _redisDb.StringGetAsync(key);

            if (redisValue.IsNullOrEmpty)
            {
                _logger.LogWarning("[Cache Miss] The key {Key} was not found on cache", key);

                return null;
            }

            _logger.LogInformation("[Cache Hit] The key {Key} was found in cache", key);

            return await _modelSerializer.DeserializeAsync<TModel>(redisValue!, cancellationToken);
        }

        public virtual async Task SetAsync<TModel>(string key, TModel value, CancellationToken cancellationToken)
            where TModel : class
        {
            using var scope = _logger.BeginScope("Trying to set {Key} key in the cache", key);

            _logger.LogInformation("Serializing the value");

            var redisValue = await _modelSerializer.SerializeAsync(value, cancellationToken);

            _logger.LogInformation("Setting the value with {Key} key in cache", key);

            var success = await _redisDb.StringSetAsync(key, redisValue, TimeSpan.FromMilliseconds(_expirationInMilliseconds));

            if (success)
                return;

            var errorTemplateMessage = "The cache set operation of {Key} key was failed";
            var logLevel = _suppressCacheSetErrors ? LogLevel.Warning : LogLevel.Error;

            _logger.Log(logLevel, errorTemplateMessage, key);

            if (!_suppressCacheSetErrors)
                throw new SetCacheOperationException(key);
        }

        public virtual async Task ExpiryAsync(string key, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope("Trying to remove {Key} key from the cache", key);

            var removed = await _redisDb.KeyDeleteAsync(key);

            if (removed)
                _logger.LogInformation("The {Key} key was removed from the cache", key);
            else
                _logger.LogWarning("The {Key} key was not removed from the cache", key);
        }

        public virtual string GenerateKey<TModel>(string suffix)
            where TModel : class
            => string.Join(':', _prefix, typeof(TModel), suffix);
    }
}
