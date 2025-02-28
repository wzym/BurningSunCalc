namespace AssistantBot.Interfaces;

public interface IIdentifierManager
{
    Task InitAsync();
    bool IsUserAllowed(long userId);
    bool IsUserAdmin(long userId);
    IReadOnlyCollection<long> GetAdminIds();
}