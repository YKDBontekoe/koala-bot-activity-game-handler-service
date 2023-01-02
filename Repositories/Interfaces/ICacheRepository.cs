namespace Koala.ActivityGameHandlerService.Repositories.Interfaces;

public interface ICacheRepository
{
    Task<string> GetAsync(string key);
    Task SetAsync(string key, string value);
    Task RemoveAsync(string key);
}