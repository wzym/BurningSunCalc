using BurningSunCalc.Interfaces;
using BurningSunCalc.Types;

namespace BurningSunCalc.AstroCalc;

public class CustomAstroCalc : IAstroCalc
{
    private const double SolsticeDeclinationDegrees = 23.44;
    private const double EclipticSunDailyAngleChangeRads = 2 * Math.PI / 365;
    private const int YearDaysBeforTheVernalEquinox = 81;

    private readonly double _latitude;
    private readonly double _longitude;

    public CustomAstroCalc(double latitude, double longitude)
    {
        _latitude = latitude;
        _longitude = longitude;
    }

    public BurningInterval CalculateLocalTimeBySolarElevation(double solarElevation, DateTime calculatingBurningIntervalDate)
    {
        var latRad = _latitude.ToRadians();
        var elevationRad = solarElevation.ToRadians();

        var declination = GetSunDeclination(calculatingBurningIntervalDate.DayOfYear);

        var cosHourAngle = (Math.Sin(elevationRad) - Math.Sin(latRad) * Math.Sin(declination))
        / (Math.Cos(latRad) * Math.Cos(declination));

        if (AchtungTimeDoesNotExist(cosHourAngle)) return new BurningInterval(DateOnly.FromDateTime(calculatingBurningIntervalDate));

        var hourAngle = Math.Acos(cosHourAngle);
        var solarFinishTime = hourAngle.ToDegrees() / 15 + 12;
        var solarStartTime = 24 - solarFinishTime;
        var resultStart = ConvertSolarTimeToLocalTime(solarStartTime, calculatingBurningIntervalDate);
        var resultFinish = ConvertSolarTimeToLocalTime(solarFinishTime, calculatingBurningIntervalDate);
        var result = new BurningInterval(
            DateOnly.FromDateTime(calculatingBurningIntervalDate), TimeOnly.FromDateTime(resultStart), TimeOnly.FromDateTime(resultFinish));

        return result;

        static bool AchtungTimeDoesNotExist(double cosHourAngle) => cosHourAngle < -1 || cosHourAngle > 1;
    }

    public decimal GetlocalSolarPowerChangeRate(int dayOfyear)
    {
        var koeff = 100 * Math.Cos(GetEclipticSunYearAngleRads(dayOfyear));
        return Convert.ToDecimal(Math.Round(koeff, 1));
    }

    public decimal GetLocalSolarPowerPercentage(int dayOfYear)
    {
        var koeff = (GetSunDelinationKoefficient(dayOfYear) + 1) / 2;
        return GetWavePercentageOfMaximum(koeff);
    }

    public double GetSunDeclination(int dayOfYear)
    {
        var resultDegrees = SolsticeDeclinationDegrees * GetSunDelinationKoefficient(dayOfYear);
        return resultDegrees.ToRadians();
    }

    private static decimal GetWavePercentageOfMaximum(double koeff)
    {
        var doubleResult = Math.Round(koeff * 100, 1);
        return Convert.ToDecimal(doubleResult);
    }

    private static double GetSunDelinationKoefficient(int dayOfYear)
        => Math.Sin(GetEclipticSunYearAngleRads(dayOfYear));

    private static double GetEclipticSunYearAngleRads(int dayOfYear)
        => EclipticSunDailyAngleChangeRads * DaysSincetheVernalEquinox(dayOfYear);

    private static int DaysSincetheVernalEquinox(int dayOfYear) => dayOfYear - YearDaysBeforTheVernalEquinox;

    private DateTime ConvertSolarTimeToLocalTime(double solarTime, DateTime date)
    {
        var equationOfTime = GetEquationOfTime(date.DayOfYear);
        var standardMeridian = 15 * Math.Round(_longitude / 15);
        var longitudeCorrection = GetLongitudeCorrection(_longitude, standardMeridian);

        var timeCorrectionFactor = longitudeCorrection + equationOfTime;

        var result = date.Date.AddHours(solarTime - timeCorrectionFactor / 60);

        if (TimeZoneInfo.Local.IsDaylightSavingTime(result)) result = result.AddHours(1);

        return result;
    }

    private static double GetEquationOfTime(int dayOfYear)
    {
        var EclipticSunYearAngleRads = GetEclipticSunYearAngleRads(dayOfYear);
        var result = 9.87 * Math.Sin(2 * EclipticSunYearAngleRads)
            - 7.53 * Math.Cos(EclipticSunYearAngleRads)
            - 1.5 * Math.Sin(EclipticSunYearAngleRads);

        return result;
    }

    private static double GetLongitudeCorrection(double longitude, double standardMeridian)
    {
        return 4 * (longitude - standardMeridian);
    }
}
