using AssistantBot.Interfaces;
using AssistantBot.Types;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace AssistantBot.Logic.StateMachine.OtherStates;

public class GetAdventureState : IState
{


    public async Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel)
    {
        var dependencies = dependenciesResolver.Get<GetAdventureDependencies>();
        var now = DateTime.Now;
        if (now < new DateTime(2025, 12, 1) || now >= new DateTime(2026, 1, 1))
        {
            await dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId,
                "Сегодня не декабрь 25-го года, придумывай приключения сам.");
            return;
        }

        await dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId,
            await dependencies.AdventuresNamesKeeper.GetForToday(now.Day));
    }
}

public class GetAdventureDependencies : IStateDependencies
{
    public static string DependencyKey => nameof(GetAdventureDependencies);

    public ITgBotClient TgBotClient { get; }
    public AdventuresNamesKeeper AdventuresNamesKeeper { get; }

    public GetAdventureDependencies(ITgBotClient tgBotClient,
        AdventuresNamesKeeper adventuresNamesKeeper)
    {
        TgBotClient = tgBotClient;
        AdventuresNamesKeeper = adventuresNamesKeeper;
    }
}

public class AdventuresNamesKeeper
{
    private const string AdventuresNamesFileName = "adventures_names.json";

    private readonly static JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
    };

    private SingleAdventure[] _adventures = [];

    public async Task InitAsync()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AdventuresNamesFileName);
        var fileContent = await File.ReadAllTextAsync(filePath)
            .ConfigureAwait(ConfigureAwaitOptions.None);
        _adventures = JsonSerializer.Deserialize<SingleAdventure[]>(fileContent)
            ?? throw new JsonException("Unable to parse a file with adventures names");
    }

    public async Task<string> GetForToday(int day)
    {
        var result = _adventures.Where(a => a.NumberOfDecember25 == day).FirstOrDefault();
        if (result is not null)
            return result.Name;

        var freeAdventures = _adventures.Where(a => a.NumberOfDecember25 == 0).ToArray();

        var rnd = new Random(DateTime.UtcNow.GetHashCode());
        var newRandomAdventure = freeAdventures[rnd.Next(freeAdventures.Length)];
        newRandomAdventure.NumberOfDecember25 = day;

        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AdventuresNamesFileName);
        await File.WriteAllTextAsync(filePath,
            JsonSerializer.Serialize(_adventures.OrderBy(a => a.Id), _options));

        return newRandomAdventure.Name;
    }
}

public class SingleAdventure
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required int NumberOfDecember25 { get; set; }
}