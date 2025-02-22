using AssistantBot.Interfaces;
using AssistantBot.Types;
using AssistantBot.Types.Dtos;
using Microsoft.Extensions.Logging;

namespace AssistantBot.Logic.Services;

public class TgBotUpdateParser : GenericUpdateMessageParser<UpdateDto>
{
    private readonly ILogger<TgBotUpdateParser> _logger;

    public TgBotUpdateParser(ILogger<TgBotUpdateParser> logger,
        ICommandsDispatcher commandsDispatcher)
        : base(commandsDispatcher)
    {
        _logger = logger;
    }

    public override UpdateModel Parse(UpdateDto update)
    {
        if (update.Message is null)
        {
            _logger.LogWarning("An {@UpdateModel} without a message", update);
            return Parse(update.CallbackQuery);
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
                Latitude = update.Message.Location.Value.Latitude,
                Longitude = update.Message.Location.Value.Longitude
            }
            : null,
            CallbackQuery = update.CallbackQuery is not null
            ? new CallbackQueryModel()
            {
                Data = update.CallbackQuery.Data
            }
            : null
        };
    }

    private UpdateModel Parse(CallbackQueryDto? callbackQuery)
    {
        if (callbackQuery is null)
            throw new Exception();

        return new UpdateModel()
        {
            ChatId = callbackQuery.Message.Chat.Id,
            Command = null,
            Text = string.Empty,
            IsCommand = false,
            CallbackQuery = new CallbackQueryModel
            {
                Data = callbackQuery.Data
            }
        };
    }
}