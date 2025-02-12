using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine.BurningSunStates;

internal class BurningSunDaysRangeRequested : BurningSunCalcBaseState
{
    public BurningSunDaysRangeRequested(BurningSunFormedRequest formedRequest)
    {
        _formedRequest = formedRequest;
    }

    protected override ValueTask ProcessSpecificly(BurningSunCalcDependencies dependenciesResolver, UpdateModel updateModel)
    {
        if (!byte.TryParse(updateModel.Text, out var parsedDaysRange))
        {
            return ValueTask.CompletedTask;
        }

        _formedRequest = _formedRequest with { WithDaysRange = parsedDaysRange };

        return ValueTask.CompletedTask;
    }
}