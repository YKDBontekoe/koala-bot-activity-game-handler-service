using Azure.Messaging.ServiceBus;
using Koala.ActivityGameHandlerService.Models.Incoming;
using Koala.ActivityGameHandlerService.Models.Outgoing;
using Koala.ActivityGameHandlerService.Options;
using Koala.ActivityGameHandlerService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Koala.ActivityGameHandlerService.Services;

public class AzureMessageService : IMessageService
{
private readonly ServiceBusClient _serviceBusClient;
    private readonly ServiceBusOptions _serviceBusOptions;
    private readonly ISteamService _steamService;
    private ServiceBusProcessor? _processor;

    public AzureMessageService(ServiceBusClient serviceBusClient, IOptions<ServiceBusOptions> serviceBusOptions, ISteamService steamService)
    {
        _serviceBusClient = serviceBusClient;
        _steamService = steamService;
        _serviceBusOptions = serviceBusOptions != null ? serviceBusOptions.Value : throw new ArgumentNullException(nameof(serviceBusOptions));
    }

    public async Task InitializeAsync()
    {
        _processor = _serviceBusClient.CreateProcessor(_serviceBusOptions.UserGameQueueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(15),
            PrefetchCount = 100,
        });
        
        try
        {
            // add handler to process messages
            _processor.ProcessMessageAsync += MessagesHandler;

            // add handler to process any errors
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task CloseQueueAsync()
    {
        if (_processor != null) await _processor.CloseAsync();
    }

    public async Task DisposeAsync()
    {
        if (_processor != null) await _processor.DisposeAsync();
    }

    private async Task MessagesHandler(ProcessMessageEventArgs args)
    {
        // Process the message.
        var body = args.Message.Body.ToString();
        
        // Implement logic to process the activity
        var sender = _serviceBusClient.CreateSender(_serviceBusOptions.ConsumerQueueName);
        
        var activity = JsonConvert.DeserializeObject<ActivityIncoming>(body);
        var activityOutgoing = new ActivityOutgoing();
        
        var GameInfo = await _steamService.GetGameInfoAsync(activity.Name);
        activityOutgoing = new ActivityOutgoing
            {
                GameInfo = GameInfo,
                Details = activity.Details,
                Name = activity.Name,
                Type = activity.Type,
                User = activity.User,
                StartedAt = activity.StartedAt
            };
            
        await sender.SendMessageAsync(new ServiceBusMessage(JsonConvert.SerializeObject(activityOutgoing)));
    }
    
    private static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // Process the error.
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}