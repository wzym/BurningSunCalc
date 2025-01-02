using System.Collections.Frozen;
using Telegram.Bot.Types;

namespace BurningSunCalc;

internal static class OwnCommands
{
    internal static IDictionary<string, string> CommandsWithDescriptions { get; } = new Dictionary<string, string> 
    {
        { "today", "Today burning time" },
        { "indays", "Burning time in a few days" },
        { "rangedays", "Burning time for the interval of days" },
        { "indaysrange", "Burning time for an interval of days beginning in the specified number of days" },
        { "setcoordinates", "Set coordinates for user" },
        { "smthelse", "another request example" }
    }.ToFrozenDictionary();

    internal static FrozenSet<BotCommand> Value { get; } = CommandsWithDescriptions
        .Select(cd => new BotCommand { Command = cd.Key, Description = cd.Value })
        .ToFrozenSet();
}