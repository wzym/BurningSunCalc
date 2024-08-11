using BurningSunCalc.TelegramBot;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

var appConfiguration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();
var secrets = new BurningSunCalcSecrets { TelegramBotSecretToken = appConfiguration["TelegramBotSecretToken"] };

var botClient = new TelegramBotClient(secrets.TelegramBotSecretToken);

botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    new ReceiverOptions { }
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
        cancellationToken: cancellationToken);
}

async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine(exception.ToString());
}

Console.ReadLine();