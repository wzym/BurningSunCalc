namespace AssistantBot.Types.Dtos;

public class MessageDto
{
    public string? Text { get; init; }
    public required ChatDto Chat { get; init; }
    public LocationDto? Location { get; init; }
}