namespace AssistantBot.Interfaces;

public interface IStateDependenciesResolver
{
    public TDependencies Get<TDependencies>() where TDependencies : IStateDependencies;
}