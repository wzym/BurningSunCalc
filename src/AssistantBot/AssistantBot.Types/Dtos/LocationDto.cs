namespace AssistantBot.Types.Dtos;

public readonly struct LocationDto
{
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
}