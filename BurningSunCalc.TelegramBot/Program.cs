using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

var botClient = new TelegramBotClient("");

botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    new ReceiverOptions()
);

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Получено сообщение '{messageText}' в чате {chatId}.");

    await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "Вы сказали:\n" + messageText,
        cancellationToken: cancellationToken)
        .ConfigureAwait(ConfigureAwaitOptions.None);
}

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine(exception.ToString());
    return Task.CompletedTask;
}

Console.ReadLine();
