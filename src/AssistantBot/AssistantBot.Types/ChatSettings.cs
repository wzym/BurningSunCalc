using BurningSunCalc.Types;

namespace AssistantBot.Types;

public class ChatSettings
{
    public static ChatSettings Default => new();
    public Coordinates? Coordinates { get; set; }
    public byte BurningSunLowestAngle { get; set; } = 37;
}