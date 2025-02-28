using System.Text.Json.Serialization;

namespace AssistantBot.Types.Dtos;

public class CallbackQueryDto
{
    [JsonPropertyName("data")]
    public string? Data { get; init; }

    [JsonPropertyName("from")]
    public required FromDto From { get; init; }

    [JsonPropertyName("message")]
    public required MessageDto Message { get; init; }
}