using Koala.ActivityGameHandlerService.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Koala.ActivityGameHandlerService;

public class SteamGameCacheUpdateWorker : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly ISteamService _steamService;

    public SteamGameCacheUpdateWorker(ISteamService steamService)
    {
        _steamService = steamService;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(UpdateCache, null, TimeSpan.Zero, TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private void UpdateCache(object state)
    {

        // Call the service here
        _steamService.UpdateGameCacheAsync();
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}