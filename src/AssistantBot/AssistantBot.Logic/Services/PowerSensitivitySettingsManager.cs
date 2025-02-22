using BurningSunCalc.AstroCalc;

namespace AssistantBot.Logic.Services;

public static class PowerSensitivitySettingsManager
{
    private static readonly byte[] AvailablePowersPercent = [60, 70, 80];
    
    public static IReadOnlyCollection<byte> GetAvailablePowersInPrecent => AvailablePowersPercent;

    public static byte GetAngleBy(byte sunPower) => LocalCalculatingExtensions.CalculateSunAngleDegreesFor(sunPower);
}