using AssistantBot.Types;

namespace AssistantBot.Interfaces;

public interface IUpdateMessageParser<in TInnerUpdate>
{
    UpdateModel Parse(TInnerUpdate update);
}

public abstract class GenericUpdateMessageParser<TInnerUpdate> : IUpdateMessageParser<TInnerUpdate>
{
    private readonly ICommandsDispatcher _commandsDispatcher;

    protected GenericUpdateMessageParser(ICommandsDispatcher commandsDispatcher)
    {
        _commandsDispatcher = commandsDispatcher;
    }

    public abstract UpdateModel Parse(TInnerUpdate update);

    protected ExtractedCommand Parse(ReadOnlySpan<char> stringInput)
    {
        stringInput = stringInput.Trim();
        if (stringInput.Length < 1) return new() { IsCommand = false, Text = string.Empty };

        if (stringInput[0] != '/') return new() { IsCommand = false, Text = stringInput.ToString() };

        var (cmdString, paramsString) = ExtractCommandAndParameters(stringInput[1..]);

        return new ExtractedCommand 
        {
            IsCommand = true,
            Command = _commandsDispatcher.Parse(cmdString),
            Text = paramsString
        };
    }

    private static (string cmdString, string paramsString) ExtractCommandAndParameters(ReadOnlySpan<char> stringInput)
    {
        for (int i = 0; i < stringInput.Length; i++)
        {
            if (char.IsWhiteSpace(stringInput[i]))
                return (stringInput[0..i].ToString(), stringInput[(i + 1)..].TrimStart().ToString());
        }

        return (stringInput.ToString(), string.Empty);
    }

    protected readonly struct ExtractedCommand
    {
        public required bool IsCommand { get; init; }
        public AssistantBotCommand? Command { get; init; }
        public required string Text { get; init; }
    }
}