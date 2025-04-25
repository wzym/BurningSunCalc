using AssistantBot.Types.TipsCalculation;

namespace AssistantBot.Logic.StateMachine.TipsCalculator;

public static class TipsCalculator
{
    public static TipsCalculationResponse Calculate(decimal invoiceAmount, 
        BanknoteDenomination minimumBanknote, TipSize tipSize)
    {
        var (rawTipsInt, rawTipsFract) = DivideByBanknote(invoiceAmount, minimumBanknote, tipSize);
        var tipsOnlySeparately = RoundByMinBanknote(minimumBanknote, rawTipsInt, rawTipsFract);

        var(rawTotalSumForOnePaymentInt, rawTotalSumForOnePaymentFract) = 
            DivideByBanknote(invoiceAmount, minimumBanknote, 100 + tipSize);
        var rawTotalSumForOnePayment = RoundByMinBanknote(
            minimumBanknote, rawTotalSumForOnePaymentInt, rawTotalSumForOnePaymentFract);

        return new TipsCalculationResponse()
        {
            TotalSumForOnePayment = rawTotalSumForOnePayment,
            TipsPartOfOnePayment = rawTotalSumForOnePayment - invoiceAmount,
            TipsOnlySeparately = tipsOnlySeparately
        };
    }

    private static (decimal intPart, decimal fractPart) DivideByBanknote(
        decimal sum, BanknoteDenomination minimumBanknote, int tipPercent)
    {
        var rawSum = sum * tipPercent / 100;
        var intPart = Math.Floor(rawSum / minimumBanknote) * minimumBanknote;
        var fractPart = rawSum - intPart;

        return (intPart, fractPart);
    }

    private static decimal RoundByMinBanknote(BanknoteDenomination minimumBanknote, decimal intPart, decimal fractPart)
        => fractPart < (minimumBanknote / 2) ? intPart : intPart + minimumBanknote;
}