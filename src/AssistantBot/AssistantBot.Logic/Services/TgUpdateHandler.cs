using AssistantBot.Interfaces;
using AssistantBot.Logic.StateMachine.BurningSunStates;
using AssistantBot.Types;
using Microsoft.Extensions.Logging;

namespace AssistantBot.Logic.Services;

public class TgUpdateHandler : ITgUpdateHandler
{
    private readonly ILogger<TgUpdateHandler> _logger;
    private readonly IStateDependenciesResolver _stateDependenciesResolver;
    private readonly IStateManager _stateManager;

    public TgUpdateHandler(ILogger<TgUpdateHandler> logger, IStateDependenciesResolver stateDependenciesResolver,
        IStateManager  stateManager)
    {
        _logger = logger;
        _stateDependenciesResolver = stateDependenciesResolver;
        _stateManager = stateManager;
    }

    public async Task Handle(UpdateModel updateModel)
    {
        if (!updateModel.IsCommand)
        {
            var currentState = _stateManager.Get(updateModel.ChatId);
            await currentState.Handle(_stateDependenciesResolver, updateModel);
            return;
        }

        var state = updateModel.Command switch
        {
            AssistantBotCommand.Today 
            or AssistantBotCommand.InDays 
            or AssistantBotCommand.DaysRange 
            or AssistantBotCommand.InDaysRange => new BurningSunStartState(),
            AssistantBotCommand.SetCoordinates => throw new NotImplementedException(),
            AssistantBotCommand.SmthElse => throw new NotImplementedException(),
            null => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };

        await state.Handle(_stateDependenciesResolver, updateModel);
    }
}