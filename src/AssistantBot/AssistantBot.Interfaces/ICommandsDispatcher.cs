using AssistantBot.Types;

namespace AssistantBot.Interfaces;

public interface ICommandsDispatcher
{
    IReadOnlyCollection<CommandModel> RegisteredCommands { get; }
    AssistantBotCommand Parse(string commandString);
}