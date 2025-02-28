namespace AssistantBot.Types;

public readonly record struct AllowedUser
{
    public required long Id { get; init; }
    public required bool IsAdmin { get; init; }
}