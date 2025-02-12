using AssistantBot.Interfaces;
using AssistantBot.Types;

namespace AssistantBot.Logic.Services;

public class ChatSettingsStore : IChatSettingsStore
{
    private readonly Dictionary<long, ChatSettings> _innerStorage = [];

    public ChatSettings Get(long chatId)
    {
        if (_innerStorage.TryGetValue(chatId, out var result))
            return result;

        result = new ChatSettings();
        _innerStorage.Add(chatId, result);
        return result;
    }

    public void Set(long chatId, ChatSettings newSettings)
    {
        _innerStorage[chatId] = newSettings;
    }
}