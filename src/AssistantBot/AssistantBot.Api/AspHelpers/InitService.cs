
using AssistantBot.Interfaces;

namespace AssistantBot.Api.AspHelpers;

internal class InitService : IHostedService
{
    private readonly ILogger<InitService> _logger;
    private readonly ITgBotWebHookConnector _webHookConnector;
    private readonly ICommandsDispatcher _commandsDispatcher;
    private readonly ITgBotClient _tgBotClient;
    private readonly ISuffMiddleageFortuneTeller _suffMiddleageFortuneTeller;

    public InitService(ILogger<InitService> logger, ITgBotWebHookConnector webHookConnector,
        ICommandsDispatcher commandsDispatcher, 
        ITgBotClient tgBotClient,
        ISuffMiddleageFortuneTeller suffMiddleageFortuneTeller)
    {
        _logger = logger;
        _webHookConnector = webHookConnector;
        _commandsDispatcher = commandsDispatcher;
        _tgBotClient = tgBotClient;
        _suffMiddleageFortuneTeller = suffMiddleageFortuneTeller;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("The app initialization is being started");

        await _suffMiddleageFortuneTeller.InitAsync();

        var registeredCommands = _commandsDispatcher.RegisteredCommands;
        await _tgBotClient.SetCommands(registeredCommands);
        await _webHookConnector.SetWebHook(cancellationToken);

        _logger.LogDebug("Commands were registered and webhook was set");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}