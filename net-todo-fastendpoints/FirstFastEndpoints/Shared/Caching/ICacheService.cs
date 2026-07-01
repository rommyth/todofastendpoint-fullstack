namespace FirstFastEndpoints.Shared.Caching;

public interface ICacheService
{
    Task<bool> KeyExistAsync(string key);

    Task<T> GetAsync<T>(string key);

    Task SetAsync<T>(string key, T value, TimeSpan expiration);

    Task RemoveAsync(string Key);
}
