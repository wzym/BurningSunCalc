using AssistantBot.Types;
using BurningSunCalc.AstroCalc;
using System.Collections.Frozen;

namespace AssistantBot.Logic.Services;

public class PowerSensitivitySettingsManager
{
    private static readonly FrozenDictionary<SunPowerSensitivity, byte> _sunDegreesInnerStorage =
        Enum.GetValues<SunPowerSensitivity>()
        .Order()
        .ToDictionary(s => s, s => LocalCalculatingExtensions.CalculateSunAngleDegreesFor(MapToSensitivityDegree(s)))
        .ToFrozenDictionary();

    public double GetAngle(SunPowerSensitivity sunPowerSensitivity) => _sunDegreesInnerStorage[sunPowerSensitivity];

    private static int MapToSensitivityDegree(SunPowerSensitivity sunPowerSteps) =>
        sunPowerSteps switch
        {
            SunPowerSensitivity.Sensitive => 60,
            SunPowerSensitivity.Neutral => 70,
            SunPowerSensitivity.ToBeBurnt => 80,
            _ => throw new Exception(),
        };
}