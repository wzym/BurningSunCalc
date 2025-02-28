namespace AssistantBot.Interfaces;

public interface ISuffMiddleageFortuneTeller
{
    string Tell(string theQuestion, long senderId);
    Task InitAsync();
}