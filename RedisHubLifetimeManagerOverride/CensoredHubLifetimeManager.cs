using Microsoft.AspNetCore.SignalR;

namespace RedisHubLifetimeManagerOverride;

public class CensoredHubLifetimeManager<THub> : HubLifetimeManager<THub> where THub : Hub
{
    private readonly HubLifetimeManager<THub> _baseManager;
    private readonly ICensoringHubProtocolFactory _protocolFactory;

    public CensoredHubLifetimeManager(HubLifetimeManager<THub> baseManager, ICensoringHubProtocolFactory protocolFactory)
    {
        _baseManager = baseManager;
        _protocolFactory = protocolFactory;
    }
    
    public override Task OnConnectedAsync(HubConnectionContext connection)
    {
        if (connection.Protocol is not CensoringHubProtocol)
            connection.Protocol = _protocolFactory.Build(connection.Protocol, connection.User);
        
        return _baseManager.OnConnectedAsync(connection);
    }

    public override Task OnDisconnectedAsync(HubConnectionContext connection) =>
        _baseManager.OnDisconnectedAsync(connection);

    public override Task SendAllAsync(string methodName, object?[] args, CancellationToken cancellationToken = new CancellationToken()) =>
        _baseManager.SendAllAsync(methodName, args, cancellationToken);

    public override Task SendAllExceptAsync(string methodName, object?[] args,
        IReadOnlyList<string> excludedConnectionIds,
        CancellationToken cancellationToken = new CancellationToken()) =>
        _baseManager.SendAllExceptAsync(methodName, args, excludedConnectionIds, cancellationToken);

    public override Task SendConnectionAsync(string connectionId, string methodName, object?[] args,
        CancellationToken cancellationToken = new CancellationToken()) =>
        _baseManager.SendConnectionAsync(connectionId, methodName, args, cancellationToken);

    public override Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object?[] args,
        CancellationToken cancellationToken = new CancellationToken()) =>
        _baseManager.SendConnectionsAsync(connectionIds, methodName, args, cancellationToken);

    public override Task SendGroupAsync(string groupName, string methodName, object?[] args,
        CancellationToken cancellationToken = new CancellationToken()) =>
        _baseManager.SendGroupAsync(groupName, methodName, args, cancellationToken);

    public override Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object?[] args,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public override Task SendGroupExceptAsync(string groupName, string methodName, object?[] args, IReadOnlyList<string> excludedConnectionIds,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public override Task SendUserAsync(string userId, string methodName, object?[] args,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public override Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object?[] args,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public override Task AddToGroupAsync(string connectionId, string groupName,
        CancellationToken cancellationToken = new CancellationToken()) =>
        _baseManager.AddToGroupAsync(connectionId, groupName, cancellationToken);

    public override Task RemoveFromGroupAsync(string connectionId, string groupName,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}