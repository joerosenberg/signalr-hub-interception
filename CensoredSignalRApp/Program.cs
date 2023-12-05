using CensoredSignalRApp;
using Microsoft.AspNetCore.SignalR;
using RedisHubLifetimeManagerOverride;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR().AddStackExchangeRedis()
    .Services.AddSingleton<HubLifetimeManager<CensoredHub>>(provider =>
    {
        var baseManager =
            new DefaultHubLifetimeManager<CensoredHub>(provider
                .GetRequiredService<ILogger<DefaultHubLifetimeManager<CensoredHub>>>());
        return new CensoredHubLifetimeManager<CensoredHub>(baseManager, new CensoringHubProtocolFactory());
    });
builder.Services.AddHostedService<DataService>();

var app = builder.Build();

app.MapGet("/test", () => Results.Ok("hello"));
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapHub<CensoredHub>("/hub");

app.Run();