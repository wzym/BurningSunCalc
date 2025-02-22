using AssistantBot.Interfaces;
using AssistantBot.Logic.Services;
using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine.SettingsStates;

public class SunAngleChangeRequestedState : IState
{
    public Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel)
    {
        var dependencies = dependenciesResolver.Get<SunAngleChangeDependencies>();
        
        var availablePowersPercent = PowerSensitivitySettingsManager.GetAvailablePowersInPrecent
            .Select(p => p.ToString())
            .ToArray();

        dependencies.StateManager.Set(updateModel.ChatId, new NewSunPowerResponseIsAwaitedState());

        return dependencies.TgBotClient.SendButtons(updateModel.ChatId, 
            "Выберите минимальную мощность солнца, от которой начинается жгучесть", 
            availablePowersPercent);
    }
}

public class SunAngleChangeDependencies : IStateDependencies
{
    public static string DependencyKey => nameof(SunAngleChangeDependencies);

    public ITgBotClient TgBotClient { get; }
    public IStateManager StateManager { get; }

    public SunAngleChangeDependencies(ITgBotClient tgBotClient,
        IStateManager stateManager)
    {
        TgBotClient = tgBotClient;
        StateManager = stateManager;
    }
}