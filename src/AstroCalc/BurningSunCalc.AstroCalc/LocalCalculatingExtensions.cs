namespace BurningSunCalc.AstroCalc;

public static class LocalCalculatingExtensions
{
    internal static double ToRadians(this double degrees)
    {
        const double RadsPerDegree = Math.PI / 180;

        return degrees * RadsPerDegree;
    }

    internal static double ToDegrees(this double radians)
    {
        const double DegreesPerRad = 180 / Math.PI;

        return radians * DegreesPerRad;
    }

    public static byte CalculateSunAngleDegreesFor(int requiredSunPowerPercent)
    {
        var angleRad = Math.Asin(requiredSunPowerPercent / 100D);
        var angleDegrees = angleRad.ToDegrees();
        var roundedResult = Math.Round(angleDegrees, 0);

        return Convert.ToByte(roundedResult);
    }
}