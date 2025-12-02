using AssistantBot.Interfaces;
using AssistantBot.Types;
using BurningSunCalc.AstroCalc;

namespace AssistantBot.Logic.Services;

public class BurningSunResponseGenerator : IBurningSunResponseGenerator
{
    public string Get(BurningSunRequest burningSunRequest)
    {
        var datesProcessed = Enumerable
            .Range(burningSunRequest.InDays, burningSunRequest.WithDaysRange)
            .Select(d => DateTime.Now.AddDays(d))
            .Select(d => new CustomAstroCalc(burningSunRequest.Coordinates).CalculateLocalTimeBySolarElevation(burningSunRequest.BurningSunLowestAngle, d))
            .ToArray();

        return string.Join("\n", datesProcessed.Select(d => d.ToString()));
    }
}