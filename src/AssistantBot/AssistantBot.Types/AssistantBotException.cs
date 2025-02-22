namespace AssistantBot.Types;

public class AssistantBotException : Exception
{
    public AssistantBotException()
    {
    }

    public AssistantBotException(string? message) : base(message)
    {
    }

    public AssistantBotException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}