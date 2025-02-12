using AssistantBot.Types;

namespace AssistantBot.Logic.StateMachine.BurningSunStates;

internal class BurningSunCoordinatesRequested : BurningSunCalcBaseState
{
    public BurningSunCoordinatesRequested(BurningSunFormedRequest formedRequest)
    {
        _formedRequest = formedRequest;
    }

    protected override async ValueTask ProcessSpecificly(BurningSunCalcDependencies dependencies, UpdateModel updateModel)
    {
        if (updateModel.Coordinates is null)
        {
            await dependencies.TgBotClient.RequestCoordinates(updateModel.ChatId, "Координаты не получены, пришлите координаты");
            return;
        }

        _formedRequest = _formedRequest with { Coordinates = updateModel.Coordinates };
        await dependencies.TgBotClient.SendCoordinatesWereReceived(updateModel.ChatId, "Координаты приняты");
        dependencies.ChatSettingsStore.Get(updateModel.ChatId).Coordinates = updateModel.Coordinates;
    }
}