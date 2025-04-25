namespace AssistantBot.Types.TipsCalculation;

public class TipsCalculationResponse
{
    public required decimal TotalSumForOnePayment { get; init; }
    public required decimal TipsPartOfOnePayment { get; init; }
    public required decimal TipsOnlySeparately { get; init; }

    public override string ToString() 
        => $"Вместе с чаевыми: {TotalSumForOnePayment},\nиз которых чаевые: {TipsPartOfOnePayment}.\n" +
        $"Чаевые отдельно: {TipsOnlySeparately}.";
}