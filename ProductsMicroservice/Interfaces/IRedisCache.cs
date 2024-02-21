namespace ProductsMicroservice.Interfaces
{
    public interface IRedisCache
    {
        Task<bool> SetAsync<T>(string key, T value, TimeSpan expiry);
        Task<T> GetAsync<T>(string key);
        Task<bool> RemoveAsync(string key);
        Task<bool> UpdateAsync<T>(string key, T value, TimeSpan expiry);
    }
}
