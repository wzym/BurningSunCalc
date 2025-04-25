using AssistantBot.Logic.StateMachine.TipsCalculator;
using AssistantBot.Types.TipsCalculation;

namespace AssistantBot.Tests.UnitTests;

public class TipsCalculatorTests
{
    [Theory]
    [InlineData(10, 1000, 50, 1100, 100, 100)]
    [InlineData(10, 1249.99, 50, 1350, 100.01, 100)]
    [InlineData(10, 1250, 50, 1400, 150, 150)]
    public void T(int tipSize, decimal sum, int minimumBanknoteValue,
        decimal totalSumExpected, decimal tipsPart, decimal tipsSeparately)
    {
        var minimumBanknote = minimumBanknoteValue switch
        {
            50 => BanknoteDenomination.Fifty,
            _ => throw new NotImplementedException()
        };

        var result = TipsCalculator.Calculate(sum, minimumBanknote, tipSize);

        Assert.Equal(totalSumExpected, result.TotalSumForOnePayment);
        Assert.Equal(tipsPart, result.TipsPartOfOnePayment);
        Assert.Equal(tipsSeparately, result.TipsOnlySeparately);
    }
}