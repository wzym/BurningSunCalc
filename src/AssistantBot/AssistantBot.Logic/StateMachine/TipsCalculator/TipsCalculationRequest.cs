using AssistantBot.Types.TipsCalculation;

namespace AssistantBot.Logic.StateMachine.TipsCalculator;

internal class TipsCalculationRequest
{
    internal BanknoteDenomination? Denomination { get; set; }
    internal TipSize? TipSize { get; set; }
    internal decimal? AccountSum { get; set; }
}