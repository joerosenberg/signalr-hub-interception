using System.Security.Claims;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace RedisHubLifetimeManagerOverride;

public interface ICensoringHubProtocolFactory
{
    CensoringHubProtocol Build(IHubProtocol baseProtocol, ClaimsPrincipal user);
}

public class CensoringHubProtocolFactory : ICensoringHubProtocolFactory
{
    private readonly Random _rnd = new();
    public CensoringHubProtocol Build(IHubProtocol baseProtocol, ClaimsPrincipal user)
    {
        var options = new CensorOptions(new Dictionary<DataCategory, int>()
        {
            { DataCategory.Alpha, _rnd.Next(5) },
            { DataCategory.Bravo, _rnd.Next(5) },
            { DataCategory.Charlie, _rnd.Next(5) },
        });
        return new CensoringHubProtocol(baseProtocol, options);
    }
}