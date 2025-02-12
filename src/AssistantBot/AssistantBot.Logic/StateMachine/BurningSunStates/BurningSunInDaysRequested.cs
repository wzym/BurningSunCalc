using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine.BurningSunStates;

internal class BurningSunInDaysRequested : BurningSunCalcBaseState
{
    public BurningSunInDaysRequested(BurningSunFormedRequest formedRequest)
    {
        _formedRequest = formedRequest;
    }

    protected override ValueTask ProcessSpecificly(BurningSunCalcDependencies dependenciesResolver, UpdateModel updateModel)
    {
        if (!byte.TryParse(updateModel.Text, out var parsedInDays))
        {
            return ValueTask.CompletedTask;
        }

        _formedRequest = _formedRequest with { InDays = parsedInDays };

        return ValueTask.CompletedTask;
    }
}