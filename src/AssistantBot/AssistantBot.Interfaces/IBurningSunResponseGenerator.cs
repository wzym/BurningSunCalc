using AssistantBot.Types;

namespace AssistantBot.Interfaces;

public interface IBurningSunResponseGenerator
{
    string Get(BurningSunRequest burningSunRequest);
}