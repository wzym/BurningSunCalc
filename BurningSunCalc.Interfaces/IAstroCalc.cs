using BurningSunCalc.Types;

namespace BurningSunCalc.Interfaces;

public interface IAstroCalc
{
    BurningInterval CalculateLocalTimeBySolarElevation(double solarElevation, DateTime calculatingBurningIntervalDate);

    decimal GetlocalSolarPowerChangeRate(int dayOfyear);

    decimal GetLocalSolarPowerPercentage(int dayOfYear);

    double GetSunDeclination(int dayOfYear);
}
