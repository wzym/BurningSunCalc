using AssistantBot.Types;

namespace AssistantBot.Interfaces;

public interface ITgUpdateHandler
{
    Task Handle(UpdateModel updateModel);
}