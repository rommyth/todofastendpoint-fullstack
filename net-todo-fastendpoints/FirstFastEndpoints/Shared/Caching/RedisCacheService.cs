using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace FirstFastEndpoints.Shared.Caching;

public class RedisCacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T> GetAsync<T>(string key)
    {
        var json = await cache.GetStringAsync(key);

        Console.WriteLine($"Key ---------------------------------- : {key}");
        Console.WriteLine($"Json: {json}");

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task<bool> KeyExistAsync(string key)
    {
        var exist = await cache.GetAsync(key);
        if (exist is not null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public async Task RemoveAsync(string key)
    {
        await cache.RemoveAsync(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        var json = JsonSerializer.Serialize(value);

        await cache.SetStringAsync(key, json, options);
    }

}
