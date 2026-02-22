using System.Collections.Concurrent;

namespace deneme.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
        private readonly ILogger<MemoryCacheService> _logger;

        // Constructor Injection
        public MemoryCacheService(ILogger<MemoryCacheService> logger)
        {
            _logger = logger;
        }

        public T? Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out var item))
            {
                if (item.ExpiresAt > DateTime.UtcNow)
                {
                    if (item.Value is T typedValue)
                    {
                        return typedValue;
                    }
                }
                else
                {
                    _cache.TryRemove(key, out _);
                    _logger.LogDebug("Cache item expired: {Key}", key);
                }
            }
            return default;
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var expiresAt = expiration.HasValue
                ? DateTime.UtcNow.Add(expiration.Value)
                : DateTime.UtcNow.AddHours(1); // Default 1 hour

            _cache.AddOrUpdate(key,
                new CacheItem { Value = value, ExpiresAt = expiresAt },
                (k, oldItem) => new CacheItem { Value = value, ExpiresAt = expiresAt });

            _logger.LogDebug("Cache item set: {Key}, expires at: {ExpiresAt}", key, expiresAt);
        }

        public void Remove(string key)
        {
            _cache.TryRemove(key, out _);
            _logger.LogDebug("Cache item removed: {Key}", key);
        }

        public void Clear()
        {
            _cache.Clear();
            _logger.LogInformation("Cache cleared");
        }

        private class CacheItem
        {
            public object Value { get; set; } = null!;
            public DateTime ExpiresAt { get; set; }
        }
    }
}

