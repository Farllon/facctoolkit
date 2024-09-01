using System;

namespace FaccToolkit.Caching.Redis
{
    public class SetCacheOperationException : Exception
    {
        public SetCacheOperationException(string? key = null) : base(key is null ? "The set cache operation was failed" : $"The set cache operation of {key} was failed")
        { }
    }
}
