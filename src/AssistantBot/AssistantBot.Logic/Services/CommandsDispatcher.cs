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
            AssistantBotCommand.Today => "Жгучее время на сегодня",
            AssistantBotCommand.InDays => "Жгучее время дня через несколько дней",
            AssistantBotCommand.DaysRange => "Жгучее время на несколько дней начиная с сегодня",
            AssistantBotCommand.InDaysRange => "Жгучее время на несколько дней через несколько дней",
            AssistantBotCommand.SetCoordinates => "Устанавливает координаты для пользователя",
            AssistantBotCommand.SetupSunAngle => "Установисть чувствительность на жгучее солнце",
            AssistantBotCommand.SmthElse => "another request example",
            AssistantBotCommand.GetSufferingPrediction => "Гадание по календарю Страдающего средневековья",
            AssistantBotCommand.CalculateTips => "Считает чаевые с разменом",
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