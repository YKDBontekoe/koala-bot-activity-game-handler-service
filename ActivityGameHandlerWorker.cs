using Koala.ActivityGameHandlerService.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Koala.ActivityGameHandlerService;

public class ActivityGameHandlerWorker : IHostedService {
    private readonly IMessageService _messageService;


    public ActivityGameHandlerWorker(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _messageService.InitializeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _messageService.DisposeAsync()!;
        await _messageService.CloseQueueAsync()!;
    }
}