using AssistantBot.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AssistantBot.Logic.Services;

public class TgBotWebHookConnector : ITgBotWebHookConnector
{
    private readonly ILogger<TgBotWebHookConnector> _logger;
    private readonly ITgBotClient _tgBotClient;
    private readonly ITgBotSecretTokenProvider _secretTokenProvider;
    private readonly IOptions<BotConfiguration> _config;

    public TgBotWebHookConnector(ILogger<TgBotWebHookConnector> logger,
        ITgBotClient tgBotClient, ITgBotSecretTokenProvider secretTokenProvider,
        IOptions<BotConfiguration> config)
    {
        _logger = logger;
        _tgBotClient = tgBotClient;
        _secretTokenProvider = secretTokenProvider;
        _config = config;
    }

    public async Task SetWebHook(CancellationToken ct = default)
    {
        var webhookUrl = _config.Value.BotWebhookUrl.AbsoluteUri;
        _logger.LogDebug("The establishing a webhook connection is started");
        await _tgBotClient.SetWebhookAsync(webhookUrl, _secretTokenProvider.Get, cancellationToken: ct);
        _logger.LogDebug("The webhook connection was established");
    }
}