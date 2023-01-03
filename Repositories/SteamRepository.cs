using System.Text.Json.Nodes;
using Koala.ActivityGameHandlerService.Models.Steam.Apps;
using Koala.ActivityGameHandlerService.Options;
using Koala.ActivityGameHandlerService.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Koala.ActivityGameHandlerService.Repositories;

public class SteamRepository : ISteamRepository
{
    private readonly ICacheRepository _cacheRepository;
    private readonly HttpClient _httpClient;
    private readonly SteamOptions _steamOptions;

    public SteamRepository(ICacheRepository cacheRepository, IOptions<SteamOptions> steamOptions)
    {
        _cacheRepository = cacheRepository;
        _steamOptions = steamOptions.Value;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(steamOptions.Value.BaseUrl);
    }

    public async Task<App> GetGameByName(string name)
    {
        var gameId = await _cacheRepository.GetAsync(name);
        if (string.IsNullOrEmpty(gameId))
        {
            throw new KeyNotFoundException($"Game with name {name} not found");
        }

        return new App
        {
            appid = ulong.TryParse(gameId, out var id) ? id : 0,
            name = name
        };
    }

    public async Task<IEnumerable<App>> GetAllAppsAsync()
    {
        var parameters =  "?key=" + _steamOptions.ApiKey + "&format=json";
        var response = await _httpClient.GetAsync("ISteamApps/GetAppList/v2/" + parameters);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<SteamGames>(content);
        ArgumentNullException.ThrowIfNull(result);

        return result.applist.apps;
    }
}