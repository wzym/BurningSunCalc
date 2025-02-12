using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine.BurningSunStates;

public class BurningSunStartState : BurningSunCalcBaseState
{
    protected override ValueTask ProcessSpecificly(BurningSunCalcDependencies dependencies, UpdateModel updateModel)
    {
        var chatSettings = dependencies.ChatSettingsStore.Get(updateModel.ChatId);
        _formedRequest = new BurningSunFormedRequest
        {
            BurningSunLowestAngle = chatSettings.BurningSunLowestAngle,
            Coordinates = chatSettings.Coordinates
        };

        _formedRequest = updateModel.Command switch
        {
            AssistantBotCommand.Today => _formedRequest with { InDays = 0, WithDaysRange = 1 },
            AssistantBotCommand.InDays => _formedRequest with { WithDaysRange = 1 },
            AssistantBotCommand.DaysRange => _formedRequest with { InDays = 0 },
            AssistantBotCommand.InDaysRange => _formedRequest,
            _ => throw new NotImplementedException(),
        };

        return ValueTask.CompletedTask;
    }
}