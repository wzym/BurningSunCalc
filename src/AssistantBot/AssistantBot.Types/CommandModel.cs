namespace AssistantBot.Types;

public readonly struct CommandModel
{
    public required string CommandString { get; init; }
    public required string Description { get; init; }
}