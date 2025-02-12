namespace AssistantBot.Interfaces;

public interface IStateManager
{
    IState Get(long chatId);
    void Set(long chatId, IState newState);
}