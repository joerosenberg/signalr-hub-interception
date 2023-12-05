using Microsoft.AspNetCore.SignalR;
using RedisHubLifetimeManagerOverride;

namespace CensoredSignalRApp;

public class DataService : IHostedService
{
    private readonly IHubContext<CensoredHub> _hubContext;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public DataService(IHubContext<CensoredHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            var rnd = new Random();
            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(2000);
                await _hubContext.Clients.All.SendAsync("Update",
                    new DomainMessageData(rnd.Next(1000).ToString(), (DataCategory)rnd.Next(3), rnd.Next(10),
                        rnd.Next(9999999).ToString()));
            }
        });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        _cts.Dispose();
        return Task.CompletedTask;
    }
}