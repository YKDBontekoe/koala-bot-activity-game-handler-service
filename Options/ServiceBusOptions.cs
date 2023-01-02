namespace Koala.ActivityGameHandlerService.Options;

public class ServiceBusOptions
{
    public const string ServiceBus = "ServiceBus";
    
    public string ConnectionString { get; set; } = string.Empty;
    public string UserGameQueueName { get; set; } = string.Empty;
    public string ConsumerQueueName { get; set; } = string.Empty;
    public string SendMessageQueueName { get; set; } = string.Empty;
}