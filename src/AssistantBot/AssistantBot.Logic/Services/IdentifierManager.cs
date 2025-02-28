using AssistantBot.Interfaces;
using AssistantBot.Types;
using Microsoft.Extensions.Logging;
using System.Collections.Frozen;
using System.Text.Json;

namespace AssistantBot.Logic.Services;

public class IdentifierManager : IIdentifierManager
{
    private const string AllowedUsersFileName = "allowedUsers.json";
    private readonly ILogger<IdentifierManager> _logger;

    private FrozenDictionary<long, bool> IdsAllowed { get; set; } = new Dictionary<long, bool>().ToFrozenDictionary();

    public IdentifierManager(ILogger<IdentifierManager> logger)
    {
        _logger = logger;
    }

    public async Task InitAsync()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AllowedUsersFileName);
        try
        {
            var fileContent = await File.ReadAllTextAsync(filePath).ConfigureAwait(ConfigureAwaitOptions.None);

            var allowedUsers = JsonSerializer.Deserialize<IReadOnlyCollection<AllowedUser>>(fileContent)
                ?? throw new AssistantBotException("Unable to parse allowed users");

            IdsAllowed = allowedUsers.ToDictionary(au => au.Id, au => au.IsAdmin)
                .ToFrozenDictionary();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to parse allowed users");
            throw new AssistantBotException("Unable to parse allowed users");
        }
    }

    public bool IsUserAllowed(long userId)
    {
        return IdsAllowed.ContainsKey(userId);
    }

    public bool IsUserAdmin(long userId)
    {
        return IdsAllowed.TryGetValue(userId, out var isAdmin) && isAdmin;
    }

    public IReadOnlyCollection<long> GetAdminIds()
    {
        return [.. IdsAllowed.Where(kvp => kvp.Value).Select(kvp => kvp.Key)];
    }
}