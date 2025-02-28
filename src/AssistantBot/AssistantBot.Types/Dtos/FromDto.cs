using System.Text.Json.Serialization;

namespace AssistantBot.Types.Dtos;

public class FromDto
{
    public required long Id { get; init; }

    [JsonPropertyName("is_bot")]
    public required bool IsBot { get; init; }

    [JsonPropertyName("first_name")]
    public required string FirstName { get; init; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }

    public required string Username { get; init; }

    [JsonPropertyName("language_code")]
    public string? LanguageCode { get; init; }
}