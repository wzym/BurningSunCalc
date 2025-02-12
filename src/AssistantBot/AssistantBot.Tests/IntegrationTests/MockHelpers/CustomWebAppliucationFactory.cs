using AssistantBot.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AssistantBot.Tests.IntegrationTests.MockHelpers;

internal class CustomWebAppliucationFactory<TEntyPoint> : WebApplicationFactory<TEntyPoint>
    where TEntyPoint : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(s =>
        {
            var tgBotWebHookConnector = s.Single(d => d.ServiceType == typeof(ITgBotWebHookConnector));
            s.Remove(tgBotWebHookConnector);
            s.AddTransient<ITgBotWebHookConnector, MockTgBotWebHookConnector>();
        });
    }
}

internal class MockTgBotWebHookConnector : ITgBotWebHookConnector
{
    public Task SetWebHook(CancellationToken ct = default) => Task.CompletedTask;
}