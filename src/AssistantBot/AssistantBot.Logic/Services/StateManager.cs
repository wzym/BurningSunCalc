using AssistantBot.Interfaces;

namespace AssistantBot.Logic.Services;

public class StateManager : IStateManager
{
    private readonly Dictionary<long, IState> _innerStorage = [];

    public IState? Get(long chatId)
    {
        return _innerStorage.GetValueOrDefault(chatId);
    }

    public void Set(long chatId, IState newState)
    {
        _innerStorage[chatId] = newState;
    }
}