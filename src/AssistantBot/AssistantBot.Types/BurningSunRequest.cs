using BurningSunCalc.Types;

namespace AssistantBot.Types;

public class BurningSunRequest
{
    public required ushort InDays { get; init; }
    public required ushort WithDaysRange { get; init; }
    public required Coordinates Coordinates { get; init; }
    public required byte BurningSunLowestAngle { get; init; }
}