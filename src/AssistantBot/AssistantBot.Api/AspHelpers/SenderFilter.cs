using AssistantBot.Interfaces;
using AssistantBot.Logic.Services;
using AssistantBot.Types;
using System.Text.Json;

namespace AssistantBot.Api.AspHelpers;

public class SenderFilter : IEndpointFilter
{
    private static readonly JsonSerializerOptions NotificationFormat = new()
    {
        WriteIndented = true
    };
    private readonly ILogger<SenderFilter> _logger;
    private readonly ITgBotClient _tgBotClient;
    private readonly UpdateModelHolder _updateModelHolder;
    private readonly IIdentifierManager _identifierManager;

    public SenderFilter(ILogger<SenderFilter> logger,
        ITgBotClient tgBotClient,
        UpdateModelHolder updateModelHolder,
        IIdentifierManager identifierManager)
    {
        _logger = logger;
        _tgBotClient = tgBotClient;
        _updateModelHolder = updateModelHolder;
        _identifierManager = identifierManager;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (_identifierManager.IsUserAllowed(_updateModelHolder.UpdateModel.FromId))
            return await next(context);

        _logger.LogWarning("An unknown user contacted us with the {@UpdateMessage}", _updateModelHolder.UpdateModel);

        var receivedWeiredMessage = JsonSerializer.Serialize(_updateModelHolder.UpdateModel, NotificationFormat);
        foreach (var adminId in _identifierManager.GetAdminIds())
        {
            await _tgBotClient.SendTextMessageAsync(adminId, $"Received message from unknown user {_updateModelHolder.UpdateModel.FromId}")
                .ConfigureAwait(ConfigureAwaitOptions.None);
            
            await _tgBotClient.SendTextMessageAsync(adminId, $"And his message is:\n{receivedWeiredMessage}")
                .ConfigureAwait(ConfigureAwaitOptions.None);
        }

        throw new AssistantBotException("A message received from an unknown user");
    }
}