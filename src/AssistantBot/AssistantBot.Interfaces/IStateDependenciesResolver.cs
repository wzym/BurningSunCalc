namespace AssistantBot.Interfaces;

public interface IStateDependenciesResolver
{
    TDependencies Get<TDependencies>() where TDependencies : IStateDependencies;
}