using BurningSunCalc.Interfaces;
using Telegram.Bot.Types;

internal class BurningSunHandler : ISubHandler
{
    public const string DependencyKey = nameof(BurningSunHandler);
    
    private readonly IAstroCalc _astroCalc;

    public BurningSunHandler(IAstroCalc astroCalc)
    {
        _astroCalc = astroCalc;
    }

    public string GenerateResponse(Message message)
    {
        var result = _astroCalc.CalculateLocalTimeBySolarElevation(37, DateTime.Now);
        return result.ToString();
    }
}