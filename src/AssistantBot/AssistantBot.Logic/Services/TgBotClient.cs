using AssistantBot.Interfaces;
using AssistantBot.Types;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AssistantBot.Logic.Services;

public class TgBotClient : ITgBotClient
{
    private readonly ILogger<TgBotClient> _logger;
    private readonly ITelegramBotClient _telegramBotClient;

    public TgBotClient(ILogger<TgBotClient> logger,
        ITelegramBotClient telegramBotClient)
    {
        _logger = logger;
        _telegramBotClient = telegramBotClient;
    }

    public Task SendTextMessageAsync(long chatId, string text)
    {
        return _telegramBotClient.SendMessage(chatId, text);
    }

    public Task SetWebhookAsync(string webhookUrl, string secretToken, CancellationToken cancellationToken)
    {
        return _telegramBotClient.SetWebhook(url: webhookUrl, secretToken: secretToken, cancellationToken: cancellationToken);
    }

    public Task RequestCoordinates(long chatId, string message)
    {
        var replyMarkup = new ReplyKeyboardMarkup()
            .AddButton(KeyboardButton.WithRequestLocation("Поделитесь локацией"));

        return _telegramBotClient.SendMessage(
            chatId: chatId,
            text: message,
            replyMarkup: replyMarkup);
    }

    public Task SendCoordinatesWereReceived(long chatId, string message)
    {
        return _telegramBotClient.SendMessage(chatId, message, replyMarkup: new ReplyKeyboardRemove());
    }

    public Task SetCommands(IReadOnlyCollection<CommandModel> commands)
    {
        var tgbCommands = commands.Select(c => new BotCommand 
        {
            Command = c.CommandString,
            Description = c.Description
        }).ToArray();

        return _telegramBotClient.SetMyCommands(tgbCommands);
    }

    public Task SendButtons(long chatId, string text, IReadOnlyCollection<string> buttonTexts)
    {
        var buttonsGenerated = buttonTexts.Select(t => InlineKeyboardButton.WithCallbackData(t, t)).ToArray();
        return _telegramBotClient.SendMessage(chatId: chatId, text: text, replyMarkup: buttonsGenerated);
    }
}