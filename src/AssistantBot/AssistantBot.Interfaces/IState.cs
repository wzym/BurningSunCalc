using AssistantBot.Types;

namespace AssistantBot.Interfaces;

public interface IState
{
    Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel);
}

public interface IStateDependencies 
{
    static abstract string DependencyKey { get; }
}