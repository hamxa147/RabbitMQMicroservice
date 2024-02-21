using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ProductsMicroservice.Interfaces;

public class RedisCacheService : IRedisCache
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan expiry)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry };
        await _cache.SetStringAsync(key, serializedValue, options);
        return true;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var serializedValue = await _cache.GetStringAsync(key);
        return serializedValue == null ? default : JsonSerializer.Deserialize<T>(serializedValue);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
        return true;
    }

    public async Task<bool> UpdateAsync<T>(string key, T value, TimeSpan expiry)
    {
        // Retrieve existing value from the cache
        var existingValue = await GetAsync<T>(key);

        if (existingValue == null)
        {
            // Item does not exist in the cache, return false
            return false;
        }

        // Update the existing value
        existingValue = value;

        // Set the updated value back into the cache
        return await SetAsync(key, existingValue, expiry);
    }
}
