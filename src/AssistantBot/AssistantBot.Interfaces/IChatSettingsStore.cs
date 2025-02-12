using AssistantBot.Types;

namespace AssistantBot.Interfaces;

public interface IChatSettingsStore
{
    public ChatSettings Get(long chatId);
    public void Set(long chatId, ChatSettings newSettings);
}