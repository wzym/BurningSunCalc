using System.Diagnostics.CodeAnalysis;

namespace AssistantBot.Types.TipsCalculation;

public interface IBanknoteDenomination
{
    decimal Denomination { get; }
}

public readonly struct BanknoteDenomination : IParsable<BanknoteDenomination>
{
    public static BanknoteDenomination Fifty => new(50);
    public static BanknoteDenomination Hundred => new(100);
    public static BanknoteDenomination TwoHundred => new(200);
    public static BanknoteDenomination FiveHundred => new(500);
    public static BanknoteDenomination OneThousand => new(1000);

    public decimal Denomination { get; }
    
    private BanknoteDenomination(decimal value)
    {
        Denomination = value;
    }

    public static implicit operator decimal(BanknoteDenomination banknote) => banknote.Denomination;

    public static BanknoteDenomination Parse(string s, IFormatProvider? provider)
    {
        return TryParse(s, provider, out var parsedResult)
            ? parsedResult 
            : throw new ArgumentException($"Unable to parse a {nameof(BanknoteDenomination)}");
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out BanknoteDenomination result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        var (parsedSuccessfully, parsedResult) = s.ToLower() switch
        {
            "50" or "fifty" => (true, Fifty),
            "100" or "hundred" => (true, Hundred),
            "200" or "twohundred" => (true, TwoHundred),
            "500" or "fivehundred" => (true, FiveHundred),
            "1000" or "onethousand" => (true, OneThousand),
            _ => (false, default),
        };

        result = parsedResult;
        return parsedSuccessfully;
    }
}