using AssistantBot.Interfaces;
using AssistantBot.Logic.StateMachine;
using AssistantBot.Logic.StateMachine.BurningSunStates;
using AssistantBot.Logic.StateMachine.Divination.SuffMiddleAge;
using AssistantBot.Logic.StateMachine.OtherStates;
using AssistantBot.Logic.StateMachine.SettingsStates;
using AssistantBot.Logic.StateMachine.TipsCalculator;
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
            if (currentState is null)
            {
                _logger.LogWarning("Unable to find a required stored state");
                currentState = new DefaultState();
            }

            await currentState.Handle(_stateDependenciesResolver, updateModel);
            return;
        }

        IState state = updateModel.Command switch
        {
            AssistantBotCommand.Today
            or AssistantBotCommand.InDays
            or AssistantBotCommand.DaysRange
            or AssistantBotCommand.InDaysRange => new BurningSunStartState(),
            AssistantBotCommand.SetCoordinates => new NewCoordinatesSetupRequested(),
            AssistantBotCommand.SmthElse => new SmthElseState(),
            AssistantBotCommand.SetupSunAngle => new SunAngleChangeRequestedState(),
            AssistantBotCommand.GetSufferingPrediction => new DivinationRequestedState(),
            AssistantBotCommand.CalculateTips => new TipsCalcBaseState(),
            _ => GetStateForUnintendedCmd(updateModel.Command),
        };

        await state.Handle(_stateDependenciesResolver, updateModel);
    }

    private DefaultState GetStateForUnintendedCmd(AssistantBotCommand? command)
    {
        _logger.LogError("Received {UnintendedCommand}", command);
        return new DefaultState();
    }
}