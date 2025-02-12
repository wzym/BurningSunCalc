using AssistantBot.Interfaces;
using AssistantBot.Types;
using System.Diagnostics.CodeAnalysis;

namespace AssistantBot.Logic.StateMachine.BurningSunStates;

public abstract class BurningSunCalcBaseState : IState
{
    protected BurningSunFormedRequest? _formedRequest;

    [MemberNotNull(nameof(_formedRequest))]
    protected abstract ValueTask ProcessSpecificly(BurningSunCalcDependencies dependenciesResolver, UpdateModel updateModel);

    public async Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel)
    {
        var dependencies = dependenciesResolver.Get<BurningSunCalcDependencies>();
        await ProcessSpecificly(dependencies, updateModel);

        if (_formedRequest.Coordinates is null)
        {
            await dependencies.TgBotClient.RequestCoordinates(updateModel.ChatId, "Пришлите координаты");
            dependencies.StateManager.Set(updateModel.ChatId, new BurningSunCoordinatesRequested(_formedRequest));
            return;
        }

        if (_formedRequest.InDays is null)
        {
            await dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId, "Через сколько дней посчитать?");
            dependencies.StateManager.Set(updateModel.ChatId, new BurningSunInDaysRequested(_formedRequest));
            return;
        }

        if (_formedRequest.WithDaysRange is null)
        {
            await dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId, "На какое количество дней посчитать?");
            dependencies.StateManager.Set(updateModel.ChatId, new BurningSunDaysRangeRequested(_formedRequest));
            return;
        }

        var result = dependencies.BurningSunResponseGenerator.Get(new BurningSunRequest()
        {
            InDays = _formedRequest.InDays.Value,
            WithDaysRange = _formedRequest.WithDaysRange.Value,
            BurningSunLowestAngle = _formedRequest.BurningSunLowestAngle.Value,
            Coordinates = _formedRequest.Coordinates.Value
        });

        await dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId, result);
        dependencies.StateManager.Set(updateModel.ChatId, new DefaultState());
    }
}

public class BurningSunCalcDependencies : IStateDependencies
{
    public ITgBotClient TgBotClient { get; }
    public IStateManager StateManager { get; }
    public IBurningSunResponseGenerator BurningSunResponseGenerator { get; }
    public IChatSettingsStore ChatSettingsStore { get; }

    public BurningSunCalcDependencies(ITgBotClient tgBotClient,
        IStateManager stateManager, IBurningSunResponseGenerator burningSunResponseGenerator,
        IChatSettingsStore chatSettingsStore)
    {
        TgBotClient = tgBotClient;
        StateManager = stateManager;
        BurningSunResponseGenerator = burningSunResponseGenerator;
        ChatSettingsStore = chatSettingsStore;
    }
}