using AssistantBot.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AssistantBot.Logic.Services;

public class StateDependenciesResolver : IStateDependenciesResolver
{
    private readonly IServiceProvider _serviceProvider;

    public StateDependenciesResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TDependencies Get<TDependencies>() where TDependencies : IStateDependencies
    {
        return _serviceProvider.GetRequiredService<TDependencies>();
    }
}