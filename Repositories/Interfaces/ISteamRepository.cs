using Koala.ActivityGameHandlerService.Models.Steam.Apps;

namespace Koala.ActivityGameHandlerService.Repositories.Interfaces;

public interface ISteamRepository : IGameRepository<App>
{
    Task<IEnumerable<App>> GetAllAppsAsync();
}