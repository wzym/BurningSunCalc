namespace AssistantBot.Interfaces;

public interface ITgBotSecretTokenProvider
{
    string Get { get; }

    void Update();
}