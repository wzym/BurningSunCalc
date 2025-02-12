using AssistantBot.Interfaces;
using AssistantBot.Types;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace AssistantBot.Logic.Services;

public class TgBotUpdateParser : GenericUpdateMessageParser<Update>
{
    private readonly ILogger<TgBotUpdateParser> _logger;

    public TgBotUpdateParser(ILogger<TgBotUpdateParser> logger,
        ICommandsDispatcher commandsDispatcher)
        : base(commandsDispatcher)
    {
        _logger = logger;
    }

    public override UpdateModel Parse(Update update)
    {
        if (update.Message is null)
        {
            throw new Exception();
        }

        var extractedCommand = Parse(update.Message.Text);

        return new UpdateModel()
        {
            ChatId = update.Message.Chat.Id,
            Command = extractedCommand.Command,
            Text = extractedCommand.Text,
            IsCommand = extractedCommand.IsCommand,
            Coordinates = update.Message.Location is not null
            ? new()
            {
                Latitude = update.Message.Location.Latitude,
                Longitude = update.Message.Location.Longitude
            }
            : null
        };
    }
}