using AssistantBot.Interfaces;

namespace AssistantBot.Api.AspHelpers;

internal sealed class RefreshingSecretService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<RefreshingSecretService> _logger;

    public RefreshingSecretService(IServiceProvider services,
        ILogger<RefreshingSecretService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            await Task.Delay(new TimeSpan(2, 0, 0), stoppingToken);
            await UpdateSecrets(stoppingToken);
            _logger.LogInformation("The secrets have been updated");
        }
    }

    private Task UpdateSecrets(CancellationToken stoppingToken)
    {
        using var scope = _services.CreateScope();
        var secretTokenProvider = scope.ServiceProvider.GetRequiredService<ITgBotSecretTokenProvider>();
        var tgBotWebHookConnector = scope.ServiceProvider.GetRequiredService<ITgBotWebHookConnector>();

        secretTokenProvider.Update();
        return tgBotWebHookConnector.SetWebHook(stoppingToken);
    }
}