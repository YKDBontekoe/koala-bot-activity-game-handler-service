using Koala.ActivityGameHandlerService.Models;
using Koala.ActivityGameHandlerService.Models.Outgoing;

namespace Koala.ActivityGameHandlerService.Services.Interfaces;

public interface ISteamService
{
    Task UpdateGameCacheAsync();
    
    Task<GameInfoOutgoing?> GetGameInfoAsync(string gameName);
}