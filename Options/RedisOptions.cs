namespace Koala.ActivityGameHandlerService.Options;

public class RedisOptions
{
    public const string Redis = "Redis";
    
    public string ConnectionString { get; set; }
    public string InstanceName { get; set; }
}