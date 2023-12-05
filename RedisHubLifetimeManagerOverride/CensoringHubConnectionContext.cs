using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace RedisHubLifetimeManagerOverride;

public class CensoringHubConnectionContext : HubConnectionContext
{
    public CensoringHubConnectionContext(ConnectionContext connectionContext, HubConnectionContextOptions contextOptions, ILoggerFactory loggerFactory) : base(connectionContext, contextOptions, loggerFactory)
    {
    }

    public override ValueTask WriteAsync(SerializedHubMessage message, CancellationToken cancellationToken = new CancellationToken())
    {
        var bytes = message.GetSerializedMessage(Protocol);
        
        return base.WriteAsync(message, cancellationToken);
    }
}