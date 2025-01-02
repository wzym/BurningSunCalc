using Telegram.Bot.Types;
using Telegram.Bot;

namespace BurningSunCalc.Interfaces;

internal interface ITgBotMessagesHandler
{
    Task Handle(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
}