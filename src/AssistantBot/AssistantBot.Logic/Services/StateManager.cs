using AssistantBot.Interfaces;

namespace AssistantBot.Logic.Services;

public class StateManager : IStateManager
{
    private readonly Dictionary<long, IState> _innerStorage = [];

    public StateManager()
    {

    }

    public IState? Get(long chatId)
    {
        if (_innerStorage.TryGetValue(chatId, out var result))
            return result;

        return null;
    }

    public void Set(long chatId, IState newState)
    {
        _innerStorage[chatId] = newState;
    }
}