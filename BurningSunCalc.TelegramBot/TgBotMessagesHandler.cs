using BurningSunCalc.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

internal class TgBotMessagesHandler : ITgBotMessagesHandler
{
    private readonly ILogger<TgBotMessagesHandler> _logger;
    private readonly ISubHandler _burningSunHandler;
    private readonly ISubHandler _smthElseHandler;

    public TgBotMessagesHandler(ILogger<TgBotMessagesHandler> logger, 
        [FromKeyedServices(BurningSunHandler.DependencyKey)] ISubHandler burningSunHandler,
        [FromKeyedServices(SmthElseHandler.DependencyKey)] ISubHandler smthElseHandler)
    {
        _logger = logger;
        _burningSunHandler = burningSunHandler;
        _smthElseHandler = smthElseHandler;
    }

    public async Task Handle(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is null || update.Message is not { } message)
            return;
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        _logger.LogDebug("Получено сообщение '{messageText}' в чате {chatId}", messageText, chatId);

        if (update.Message?.From?.Id is not (111799970 or 71984212))
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: "А ты кто такой?",
                cancellationToken: cancellationToken)
                .ConfigureAwait(ConfigureAwaitOptions.None);
            return;
        }

        var responseResult = messageText switch
        {
            var commandValue when commandValue.StartsWith("/today") => _burningSunHandler.GenerateResponse(update.Message),
            var commandValue when commandValue.StartsWith("/smthelse") => _smthElseHandler.GenerateResponse(update.Message),
            _ => "Команда не опознана",
        };
        
        await botClient.SendMessage(
            chatId: chatId,
            text: responseResult,
            cancellationToken: cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);
    }
}