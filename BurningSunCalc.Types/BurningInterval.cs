namespace BurningSunCalc.Types;

public record BurningInterval
{
    public bool Exists { get; }

    public DateOnly Date { get; }

    public TimeOnly Start { get; }

    public TimeOnly Finish { get; }

    public TimeSpan AchtungIntervalLength => Finish - Start;

    public BurningInterval(DateOnly date, TimeOnly start, TimeOnly finish)
    {
        Exists = true;
        Date = date;
        Start = start;
        Finish = finish;
    }

    public BurningInterval(DateOnly date)
    {
        Exists = false;
        Date = date;
    }

    public override string ToString() => $"{Date:dd MMM ddd}:\t{{ {Start:HH:mm} - {Finish:HH:mm} }} ▒ {IntervalLengthToString()}";

    private string IntervalLengthToString()
    {
        var intLength = AchtungIntervalLength;
        return $"{intLength.Hours:00}:{intLength.Minutes:00}";
    }
}
