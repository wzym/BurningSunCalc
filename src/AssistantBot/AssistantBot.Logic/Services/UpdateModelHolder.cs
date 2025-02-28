using AssistantBot.Types;

namespace AssistantBot.Logic.Services;

public class UpdateModelHolder
{
    public UpdateModel UpdateModel { get; set; } = new UpdateModel() 
    {
        ChatId = 0,
        Command = null,
        FromId = 0,
        IsCommand = false,
        Text = string.Empty
    };
}