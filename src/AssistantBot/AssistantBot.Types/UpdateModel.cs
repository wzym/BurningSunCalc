using BurningSunCalc.Types;
using System.Diagnostics.CodeAnalysis;

namespace AssistantBot.Types;

public class UpdateModel
{
    public required long ChatId { get; init; }

    [MemberNotNullWhen(true, nameof(Command))]
    public required bool IsCommand { get; init; }

    public required AssistantBotCommand? Command { get; init; }

    public required string Text { get; init; }

    public Coordinates? Coordinates { get; init; }

    public CallbackQueryModel? CallbackQuery { get; init; }
}