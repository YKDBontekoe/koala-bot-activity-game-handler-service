using System.Text;
using Koala.ActivityGameHandlerService.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Koala.ActivityGameHandlerService.Repositories;

public class RedisRepository : ICacheRepository
{
    private readonly IDistributedCache _cache;

    public RedisRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<string> GetAsync(string key)
    {
        var cacheResult = await _cache.GetAsync(key);
        return cacheResult is null ? string.Empty : Encoding.UTF8.GetString(cacheResult);
    }

    public Task SetAsync(string key, string value)
    {
        var cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
        return _cache.SetAsync(key, Encoding.UTF8.GetBytes(value), cacheEntryOptions);
    }

    public Task RemoveAsync(string key)
    {
        return _cache.RemoveAsync(key);
    }
}