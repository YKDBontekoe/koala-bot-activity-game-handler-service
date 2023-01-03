using Koala.ActivityGameHandlerService.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Koala.ActivityGameHandlerService;

public class SteamGameCacheUpdateWorker : IHostedService
{
    private Timer _timer;
    private readonly ISteamService _steamService;

    public SteamGameCacheUpdateWorker(ISteamService steamService)
    {
        _steamService = steamService;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(async _ => await UpdateCache(), null, TimeSpan.FromMinutes(1), TimeSpan.FromHours(24));
        
        return Task.CompletedTask;
    }

    private Task UpdateCache()
    {
        // Call the service here
        return _steamService.UpdateGameCacheAsync();
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
}