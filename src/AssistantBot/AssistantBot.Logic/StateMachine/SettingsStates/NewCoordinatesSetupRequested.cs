using AssistantBot.Interfaces;
using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine.SettingsStates;

public class NewCoordinatesSetupRequested : IState
{
    public Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel)
    {
        var dependencies = dependenciesResolver.Get<NewCoordinatesSetupDependencies>();

        dependencies.StateManager.Set(updateModel.ChatId, new NewCoordinatesAwaitedState());
        return dependencies.TgBotClient.RequestCoordinates(updateModel.ChatId, "Введите новые координаты");
    }
}

public class NewCoordinatesSetupDependencies : IStateDependencies
{
    public static string DependencyKey => nameof(NewCoordinatesSetupDependencies);

    public ITgBotClient TgBotClient { get; }
    public IStateManager StateManager { get; }

    public NewCoordinatesSetupDependencies(ITgBotClient tgBotClient,
        IStateManager stateManager)
    {
        TgBotClient = tgBotClient;
        StateManager = stateManager;
    }
}