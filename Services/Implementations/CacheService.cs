using Microsoft.Extensions.Caching.Memory;
using Sub_Pal_API.Services.Interfaces;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Sub_Pal_API.Services.Implementations;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ConcurrentDictionary<string, byte> _keys;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _keys = new ConcurrentDictionary<string, byte>();
    }

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        if (_memoryCache.TryGetValue(key, out T? value))
        {
            return Task.FromResult(value);
        }

        return Task.FromResult<T?>(null);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var cacheExpiration = expiration ?? _defaultExpiration;

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(cacheExpiration)
            .RegisterPostEvictionCallback((evictedKey, evictedValue, reason, state) =>
            {
                // Remove from keys tracking
                _keys.TryRemove(evictedKey.ToString()!, out _);
            });

        _memoryCache.Set(key, value, cacheEntryOptions);
        _keys.TryAdd(key, 0);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        _keys.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        // Convert pattern to regex-like matching
        // e.g., "user:123:*" becomes all keys starting with "user:123:"
        var prefix = pattern.Replace("*", "");
        
        var matchingKeys = _keys.Keys.Where(k => k.StartsWith(prefix)).ToList();
        
        foreach (var key in matchingKeys)
        {
            _memoryCache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(_memoryCache.TryGetValue(key, out _));
    }
}
