namespace Koala.ActivityGameHandlerService.Services.Interfaces;

public interface IMessageService
{
    Task InitializeAsync();
    Task CloseQueueAsync();
    Task DisposeAsync();
}