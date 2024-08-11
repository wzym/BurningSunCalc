namespace BurningSunCalc.Types.ErrorProcessing;

public class BurningSunCalcException : Exception
{
    public BurningSunCalcErrorType ErrorType { get; }

    public BurningSunCalcException(BurningSunCalcErrorType errorType, string message)
        : base(message)
    {
        ErrorType = errorType;
    }
}
