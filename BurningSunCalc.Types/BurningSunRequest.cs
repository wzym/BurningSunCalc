namespace BurningSunCalc.Types;

public readonly record struct BurningSunRequest
{
    public static BurningSunRequest Default => new() { InDays = 0, WithDaysRange = 1 };

    public required ushort InDays { get; init; }
    public required ushort WithDaysRange { get; init; }
}