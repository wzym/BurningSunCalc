using BurningSunCalc.AstroCalc;

namespace BurningSunCalc.UnitTests;

public class CalculateSunAngleDegreesTests
{
    [Theory]
    [InlineData(60, 37)]
    [InlineData(70, 44)]
    [InlineData(80, 53)]
    public void CorrectlyCalculatesRequiredAngleForRequestedSunPower(int givenPowerPercent, byte expectedResult)
    {
        var a = LocalCalculatingExtensions.CalculateSunAngleDegreesFor(givenPowerPercent);
        Assert.Equal(expectedResult, a);
    }
}