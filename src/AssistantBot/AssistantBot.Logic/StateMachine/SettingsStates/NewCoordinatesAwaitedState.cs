using AssistantBot.Interfaces;
using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine.SettingsStates;

internal class NewCoordinatesAwaitedState : IState
{
    public async Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel)
    {
        var dependencies = dependenciesResolver.Get<NewCoordinatesAwaitedDependencies>();

        if (updateModel.Coordinates is null)
        {
            await dependencies.TgBotClient.RequestCoordinates(updateModel.ChatId, "Координаты не получены, пришлите координаты");
            return;
        }

        await dependencies.TgBotClient.SendCoordinatesWereReceived(updateModel.ChatId, "Координаты приняты");
    }
}

public class NewCoordinatesAwaitedDependencies : IStateDependencies
{
    public static string DependencyKey => nameof(NewCoordinatesAwaitedDependencies);

    public ITgBotClient TgBotClient { get; }

    public NewCoordinatesAwaitedDependencies(ITgBotClient tgBotClient)
    {
        TgBotClient = tgBotClient;
    }
}