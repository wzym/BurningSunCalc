using Telegram.Bot.Types;

internal interface ISubHandler
{
    string GenerateResponse(Message message);
}