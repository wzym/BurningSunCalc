using AssistantBot.Interfaces;
using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine;

public class DefaultState : IState
{
    public Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel)
    {
        var dependencies = dependenciesResolver.Get<DefaultStateDependencies>();

        return dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId, 
            "Что-то не так пошло, вероятно, из-за перезапусков нарушилась консистентность");
    }
}

public class DefaultStateDependencies : IStateDependencies
{
    public static string DependencyKey => nameof(DefaultStateDependencies);

    public ITgBotClient TgBotClient { get; }

    public DefaultStateDependencies(ITgBotClient tgBotClient)
    {
        TgBotClient = tgBotClient;
    }
}