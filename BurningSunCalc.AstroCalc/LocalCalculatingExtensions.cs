namespace BurningSunCalc.AstroCalc;

internal static class LocalCalculatingExtensions
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
}
