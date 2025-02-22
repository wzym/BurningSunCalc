using AssistantBot.Types;

namespace AssistantBot.Interfaces;

public interface ITgBotClient
{
    Task SendTextMessageAsync(long chatId, string text);
    Task SetWebhookAsync(string webhookUrl, string secretToken, CancellationToken cancellationToken);
    Task RequestCoordinates(long chatId, string message);
    Task SendCoordinatesWereReceived(long chatId, string message);
    Task SetCommands(IReadOnlyCollection<CommandModel> commands);
    Task SendButtons(long chatId, string text, IReadOnlyCollection<string> buttonTexts);
}