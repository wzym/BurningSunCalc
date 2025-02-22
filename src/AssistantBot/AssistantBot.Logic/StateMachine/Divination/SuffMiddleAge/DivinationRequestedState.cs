using AssistantBot.Interfaces;
using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine.Divination.SuffMiddleAge;

public class DivinationRequestedState : IState
{
    public Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel)
    {
        var dependencies = dependenciesResolver.Get<DivinationRequestedDependencies>();

        var prediction = dependencies.SuffMiddleageFortuneTeller.Tell();
        var predictionResponse = $"Спрошено: \"{updateModel.Text}\"\nОтвет: \"{prediction}\""; 
        return dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId, predictionResponse);
    }
}

public class DivinationRequestedDependencies : IStateDependencies
{
    public static string DependencyKey => nameof(DivinationRequestedDependencies);

    public ITgBotClient TgBotClient { get; }

    public ISuffMiddleageFortuneTeller SuffMiddleageFortuneTeller { get; }

    public DivinationRequestedDependencies(ITgBotClient tgBotClient,
        ISuffMiddleageFortuneTeller suffMiddleageFortuneTeller)
    {
        TgBotClient = tgBotClient;
        SuffMiddleageFortuneTeller = suffMiddleageFortuneTeller;
    }
}