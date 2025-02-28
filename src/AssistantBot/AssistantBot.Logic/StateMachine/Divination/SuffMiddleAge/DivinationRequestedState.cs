using AssistantBot.Interfaces;
using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine.Divination.SuffMiddleAge;

public class DivinationRequestedState : IState
{
    public Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel)
    {
        var dependencies = dependenciesResolver.Get<DivinationRequestedDependencies>();
        
        if (updateModel.Text == string.Empty)
        {
            dependencies.StateManager.Set(updateModel.ChatId, this);
            return dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId, "Вопросик задай");
        }

        var prediction = dependencies.SuffMiddleageFortuneTeller.Tell(updateModel.Text, updateModel.FromId);
        var predictionResponse = $"Спрошено: \"{updateModel.Text}\"\nОтвет: \"{prediction}\""; 
        return dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId, predictionResponse);
    }
}

public class DivinationRequestedDependencies : IStateDependencies
{
    public static string DependencyKey => nameof(DivinationRequestedDependencies);

    public ITgBotClient TgBotClient { get; }

    public ISuffMiddleageFortuneTeller SuffMiddleageFortuneTeller { get; }

    public IStateManager StateManager { get; }

    public DivinationRequestedDependencies(ITgBotClient tgBotClient,
        ISuffMiddleageFortuneTeller suffMiddleageFortuneTeller,
        IStateManager stateManager)
    {
        TgBotClient = tgBotClient;
        SuffMiddleageFortuneTeller = suffMiddleageFortuneTeller;
        StateManager = stateManager;
    }
}