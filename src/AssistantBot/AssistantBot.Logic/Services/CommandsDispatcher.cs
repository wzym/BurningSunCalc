using System.Collections.Frozen;
using System.Text;
using AssistantBot.Interfaces;
using AssistantBot.Types;
using Microsoft.Extensions.Logging;

namespace AssistantBot.Logic.Services;

public class CommandsDispatcher : ICommandsDispatcher
{
    private const char CommandSeparator = '_';
    
    private static readonly FrozenDictionary<string, AssistantBotCommand> InnerCommandsStorage = GetCommands();
    
    private readonly ILogger<CommandsDispatcher> _logger;

    public IReadOnlyCollection<CommandModel> RegisteredCommands => [.. InnerCommandsStorage
        .Select(e => new CommandModel
        {
            CommandString = e.Key,
            Description = GetDescription(e.Value)
        })];

    public CommandsDispatcher(ILogger<CommandsDispatcher> logger)
    {
        _logger = logger;
    }

    public AssistantBotCommand Parse(string commandString)
    {
        if (!InnerCommandsStorage.TryGetValue(commandString, out var enumResult))
        {
            _logger.LogWarning("Requested a not parsable command {CommandString}", commandString);
            throw new ArgumentException($"Unable to find the command '{commandString}'");
        }

        return enumResult;
    }

    private static string GetDescription(AssistantBotCommand command) => 
        command switch 
        {     
            AssistantBotCommand.Today => "Today burning time",     
            AssistantBotCommand.InDays => "Burning time in a few days",     
            AssistantBotCommand.DaysRange => "Burning time for the interval of days",     
            AssistantBotCommand.InDaysRange => "Burning time for an interval of days beginning in the specified number of days",     
            AssistantBotCommand.SetCoordinates => "Set coordinates for user",
            AssistantBotCommand.SetupSunAngle => "Set sensitivity for the burning sun calculating",
            AssistantBotCommand.SmthElse => "another request example",
            AssistantBotCommand.GetSufferingPrediction => "Returns a prediction by suffering middleage calendar",
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
        };
    
    private static FrozenDictionary<string,AssistantBotCommand> GetCommands()
    {
        return Enum.GetValues<AssistantBotCommand>()
            .ToDictionary(GetCommandString, ec => ec)
            .ToFrozenDictionary();
    }
    
    private static string GetCommandString<TCmd>(TCmd enumCommand) 
        where TCmd : struct, Enum
    {
        var commandName = enumCommand.ToString();
        var sb = new StringBuilder();
        sb.Append(char.ToLower(commandName[0]));
        for (var i = 1; i < commandName.Length; i++)
        {
            if (char.IsUpper(commandName[i]))
            {
                sb.Append(CommandSeparator);
                sb.Append(char.ToLower(commandName[i]));
            }
            else
            {
                sb.Append(commandName[i]);
            }
        }

        return sb.ToString();
    }
}