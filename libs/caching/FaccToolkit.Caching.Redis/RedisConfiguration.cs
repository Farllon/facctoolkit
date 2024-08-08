namespace FaccToolkit.Caching.Redis
{
    public class RedisConfiguration
    {
        public int Database { get; set; } = -1;

        public string Prefix { get; set; } = string.Empty;

        public int ExpirationInMilliseconds { get; set; } = -1;

        public bool SuppressCacheSetErrors { get; set; } = true;
    }
}
