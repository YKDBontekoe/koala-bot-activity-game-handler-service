namespace Koala.ActivityGameHandlerService.Repositories.Interfaces;

public interface IGameRepository<T>
{
    Task<T> GetGameByName(string name);
}