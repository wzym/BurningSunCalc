namespace BurningSunCalc.Types;

public readonly record struct Coordinates
{
    public required double Longitude { get; init; }

    public required double Latitude { get; init; }
}