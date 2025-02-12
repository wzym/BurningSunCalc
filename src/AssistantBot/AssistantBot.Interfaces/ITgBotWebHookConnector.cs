namespace AssistantBot.Interfaces;

public interface ITgBotWebHookConnector
{
    Task SetWebHook(CancellationToken ct = default);
}