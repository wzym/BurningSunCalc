using System.Diagnostics.CodeAnalysis;

namespace BurningSunCalc.TelegramBot;

internal interface IBurningSunCalcSecrets
{
    string TelegramBotSecretToken { get; init; }
}

internal class BurningSunCalcSecrets : IBurningSunCalcSecrets
{
    private string _telegramBotSecretToken;

    [AllowNull]
    public required string TelegramBotSecretToken
    {
        get => _telegramBotSecretToken;
        [MemberNotNull(nameof(_telegramBotSecretToken))]
        init => _telegramBotSecretToken = value ?? string.Empty;
    }
}
