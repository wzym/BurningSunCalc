namespace AssistantBot.Logic;

public class BotConfiguration
{
    public required string BotToken { get; init; }
    public required Uri BotWebhookUrl { get; init; }
}