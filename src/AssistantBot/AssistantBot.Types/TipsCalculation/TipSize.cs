namespace AssistantBot.Types.TipsCalculation;

public readonly struct TipSize
{
    public static TipSize Low => new(7);
    public static TipSize Medium => new(10);
    public static TipSize High => new(13);

    public int ValuePercent { get; }

    private TipSize(int valuePercent)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(valuePercent, 0);
        ValuePercent = valuePercent;
    }

    public static implicit operator int(TipSize tipSize) => tipSize.ValuePercent;
    public static implicit operator TipSize(int tipSize) => new(tipSize);
}