using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace RedisHubLifetimeManagerOverride;

public record CensorOptions(Dictionary<DataCategory, int> HighestAllowedLevel)
{
    public override string ToString()
    {
        return string.Join('-',
            HighestAllowedLevel.Keys.Order().Select(key => $"{key}-{HighestAllowedLevel[key]}"));
    }
}

public class CensoringHubProtocol : IHubProtocol
{
    private readonly IHubProtocol _baseProtocol;
    private readonly CensorOptions _options;

    public CensoringHubProtocol(IHubProtocol baseProtocol, CensorOptions options)
    {
        _baseProtocol = baseProtocol;
        _options = options;
    }
    
    public bool TryParseMessage(ref ReadOnlySequence<byte> input, IInvocationBinder binder, [NotNullWhen(true)] out HubMessage? message)
    {
        return _baseProtocol.TryParseMessage(ref input, binder, out message);
    }

    public void WriteMessage(HubMessage message, IBufferWriter<byte> output)
    {
        _baseProtocol.WriteMessage(CensorMessage(message), output);
    }
    
    public ReadOnlyMemory<byte> GetMessageBytes(HubMessage message)
    {
        return _baseProtocol.GetMessageBytes(CensorMessage(message));
    }

    private HubMessage CensorMessage(HubMessage message) => message switch
    {
        InvocationMessage msg => new InvocationMessage(msg.InvocationId, msg.Target,
            msg.Arguments.Select(GetCensoredObject).ToArray()),
        StreamInvocationMessage msg => new StreamInvocationMessage(msg.InvocationId!, msg.Target,
            msg.Arguments.Select(GetCensoredObject).ToArray()),
        _ => message
    };

    private object? GetCensoredObject(object? arg)
    {
        if (arg is not DomainMessageData data) return arg;
        
        var highestAllowedLevel = _options.HighestAllowedLevel[data.Category];
        if (data.Level > highestAllowedLevel) return data with { SensitiveData = $"Highest allowed level is {highestAllowedLevel}" };
        return data;
    }

    public bool IsVersionSupported(int version)
    {
        return _baseProtocol.IsVersionSupported(version);
    }

    // Name must be unique across different protocols, otherwise 
    public string Name => $"{_baseProtocol.Name}-censored-{_options}";
    public int Version => _baseProtocol.Version;
    public TransferFormat TransferFormat => _baseProtocol.TransferFormat;
}