using AssistantBot.Interfaces;
using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine.OtherStates;

public class SmthElseState : IState
{
    private static readonly string[] SmthElseResponses = ["ёб", "пизда", "хуй", "блядь"];

    public Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel)
    {
        var dependencies = dependenciesResolver.Get<SmthElseStateDependencies>();

        return dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId, SmthElseResponses[Random.Shared.Next(SmthElseResponses.Length - 1)]);
    }
}

public class SmthElseStateDependencies : IStateDependencies
{
    public static string DependencyKey => nameof(SmthElseStateDependencies);

    public ITgBotClient TgBotClient { get; }

    public SmthElseStateDependencies(ITgBotClient tgBotClient)
    {
        TgBotClient = tgBotClient;
    }
}