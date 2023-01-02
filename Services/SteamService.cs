using System.Text;
using Koala.ActivityGameHandlerService.Models;
using Koala.ActivityGameHandlerService.Models.Outgoing;
using Koala.ActivityGameHandlerService.Repositories.Interfaces;
using Koala.ActivityGameHandlerService.Services.Interfaces;

namespace Koala.ActivityGameHandlerService.Services;

public class SteamService : ISteamService
{
    private readonly ICacheRepository _cacheRepository;
    private readonly ISteamRepository _steamRepository;
    private readonly IScraperRepository _scraperRepository;

    public SteamService(ICacheRepository cacheRepository, ISteamRepository steamRepository, IScraperRepository scraperRepository)
    {
        _cacheRepository = cacheRepository;
        _steamRepository = steamRepository;
        _scraperRepository = scraperRepository;
    }
    

    public Task UpdateGameCacheAsync()
    {
        return Task.Run(async () =>
        {
            var games = await _steamRepository.GetAllAppsAsync();
            foreach (var game in games)
            {
                await _cacheRepository.SetAsync(game.name, game.appid.ToString());
            }
        });
    }

    public async Task<GameInfoOutgoing?> GetGameInfoAsync(string gameName)
    {
        var gameInfo = new GameInfoOutgoing();
        var appId = await _steamRepository.GetGameByName(gameName);
        if (appId == null)
        {
            return null;
        }
        
        var sb = new StringBuilder();
        sb.Append("https://store.steampowered.com/app/");
        sb.Append(appId);
        var gameUrl = sb.ToString();
        var game = await _scraperRepository.GetGameDataFromUrl(gameUrl);

        gameInfo.Developers = game.Developers;
        gameInfo.Tags = game.Tags;
        gameInfo.Price = game.Price;
        gameInfo.ReleaseDate = game.ReleaseDate;
        gameInfo.RecentReviewScore = game.RecentReviewScore;
        gameInfo.AllTimeReviewScore = game.AllTimeReviewScore;
        
        return gameInfo;
    }
}