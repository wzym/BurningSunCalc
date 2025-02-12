using BurningSunCalc.Types;

namespace AssistantBot.Logic.StateMachine.BurningSunStates;

public record BurningSunFormedRequest
{
    internal ushort? InDays { get; init; }

    internal ushort? WithDaysRange { get; init; }

    internal Coordinates? Coordinates { get; init; }

    internal byte? BurningSunLowestAngle { get; init; }
}