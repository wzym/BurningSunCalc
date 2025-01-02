using Telegram.Bot.Types;

internal class SmthElseHandler : ISubHandler
{
    public const string DependencyKey = nameof(SmthElseHandler);

    private static readonly string[] _innerValues = ["ёб", "хуй", "пизда", "блядь"];

    public string GenerateResponse(Message message)
    {
        return new Random().GetItems(_innerValues, 1).First();
    }
}