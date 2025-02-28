using AssistantBot.Interfaces;

namespace AssistantBot.Logic.Services;

public class SuffMiddleageFortuneTeller : ISuffMiddleageFortuneTeller
{
    private const string PredictionsFileName = "predictions_set_1.txt";

    private static string[] Predictions = [];

    public SuffMiddleageFortuneTeller()
    {

    }

    public async Task InitAsync()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PredictionsFileName);
        var linesReceived = await File.ReadAllLinesAsync(filePath)
            .ConfigureAwait(ConfigureAwaitOptions.None);
        Predictions = linesReceived;
    }

    public string Tell(string theQuestion, long senderId)
    {
        var rnd = new Random(HashCode.Combine(theQuestion, DateTime.UtcNow));

        var randomIndex = rnd.Next(Predictions.Length);
        return Predictions[randomIndex];
    }
}