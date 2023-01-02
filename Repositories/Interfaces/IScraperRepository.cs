using Koala.ActivityGameHandlerService.Models.Steam;

namespace Koala.ActivityGameHandlerService.Repositories.Interfaces;

public interface IScraperRepository
{
    Task<GameData> GetGameDataFromUrl(string url);
}