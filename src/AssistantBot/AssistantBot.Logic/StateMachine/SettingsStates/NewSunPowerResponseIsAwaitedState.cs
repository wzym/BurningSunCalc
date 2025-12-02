using AssistantBot.Interfaces;
using AssistantBot.Logic.Services;
using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine.SettingsStates;

internal class NewSunPowerResponseIsAwaitedState : IState
{
    public Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel)
    {
        var dependencies = dependenciesResolver.Get<NewSunPowerRespIsAwaitedDependencies>();
        var requiredPowerReceived = updateModel.CallbackQuery is null
            ? updateModel.Text
            : updateModel.CallbackQuery.Data;
        requiredPowerReceived ??= "null";
        
        if (!byte.TryParse(requiredPowerReceived, out var parsedPowerRequired))
            return dependencies.TgBotClient.SendTextMessageAsync(
                updateModel.ChatId,
                $"Не удалось считать нужную мощность из \'{requiredPowerReceived}\'");
        
        var newRequiredAngle = PowerSensitivitySettingsManager.GetAngleBy(parsedPowerRequired);
        dependencies.ChatSettingsStore.Get(updateModel.ChatId).BurningSunLowestAngle = newRequiredAngle;

        return dependencies.TgBotClient.SendTextMessageAsync(
                updateModel.ChatId,
                $"Установлен новый угол жгучего солнца: \'{newRequiredAngle}\'");
    }
}

public class NewSunPowerRespIsAwaitedDependencies : IStateDependencies
{
    public static string DependencyKey => nameof(NewSunPowerRespIsAwaitedDependencies);

    public IChatSettingsStore ChatSettingsStore { get; }
    public ITgBotClient TgBotClient { get; }

    public NewSunPowerRespIsAwaitedDependencies(IChatSettingsStore chatSettingsStore, 
        ITgBotClient tgBotClient)
    {
        ChatSettingsStore = chatSettingsStore;
        TgBotClient = tgBotClient;
    }
}