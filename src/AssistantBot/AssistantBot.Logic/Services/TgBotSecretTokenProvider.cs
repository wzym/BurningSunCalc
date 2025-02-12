using AssistantBot.Interfaces;

namespace AssistantBot.Logic.Services;

public class TgBotSecretTokenProvider : ITgBotSecretTokenProvider
{
    public string Get { get; private set; }

    public TgBotSecretTokenProvider()
    {
        Get = GenerateNew();
    }

    public void Update() => Get = GenerateNew();

    private static string GenerateNew()
    {
        return $"{Guid.NewGuid()}-{Guid.NewGuid()}";
    }
}