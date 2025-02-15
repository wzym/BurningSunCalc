using AssistantBot.Logic.Services;
using AssistantBot.Types;

namespace AssistantBot.Tests.UnitTests;

public class PowerSensitivitySettingsManagerTests
{
    [Theory]
    [InlineData(SunPowerSensitivity.Sensitive, 37)]
    [InlineData(SunPowerSensitivity.Neutral, 44)]
    [InlineData(SunPowerSensitivity.ToBeBurnt, 53)]
    public void CorrectlyMapsSunPowerSensitivityToSunAngle(SunPowerSensitivity givenSunPower, byte expectedResult)
    {
        var result = new PowerSensitivitySettingsManager().GetAngle(givenSunPower);
        Assert.Equal(expectedResult, result);
    }
}