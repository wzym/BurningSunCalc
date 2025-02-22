using System.Text.Json.Serialization;

namespace AssistantBot.Types.Dtos;

public class UpdateDto
{
    [JsonPropertyName("update_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public required int Id { get; init; }

    public MessageDto? Message { get; init; }

    [JsonPropertyName("callback_query")]
    public CallbackQueryDto? CallbackQuery { get; init; }
}