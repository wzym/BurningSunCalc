using AssistantBot.Interfaces;
using AssistantBot.Types;
using AssistantBot.Types.TipsCalculation;
using System.Globalization;

namespace AssistantBot.Logic.StateMachine.TipsCalculator;

public class TipsCalcBaseState : IState
{
    private readonly TipsCalculationRequest _tipsCalculationRequest = new();
    private CurrentState _state = CurrentState.Start;

    public async Task Handle(IStateDependenciesResolver dependenciesResolver, UpdateModel updateModel)
    {
        var dependencies = dependenciesResolver.Get<TipsCalcDependencies>();
        dependencies.StateManager.Set(updateModel.ChatId, this);

        switch (_state)
        {
            case CurrentState.SumRequested:
            {
                if (await ProcessSumReceived(updateModel, dependencies)) return;
                break;
            }
            case CurrentState.TipSizeRequested:
            {
                if (await ProcessTipSizeReceived(updateModel, dependencies)) return;
                break;
            }
            case CurrentState.DenominationRequested:
            {
                if (await ProcessDenominationReceived(updateModel, dependencies)) return;
                break;
            }
            case CurrentState.Start:
                break;
            default:
                throw new AssistantBotException($"Wrong state \'{_state}\' in \'{nameof(TipsCalcBaseState)}\'");
        }

        if (_tipsCalculationRequest.AccountSum is null)
        {
            _state = CurrentState.SumRequested;
            await dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId, "С какой суммы считаем?");
            return;
        }

        if (_tipsCalculationRequest.Denomination is null)
        {
            _state = CurrentState.DenominationRequested;
            await dependencies.TgBotClient.SendButtons(updateModel.ChatId, "Для какой минимальной купюры считать?",
                [ "50", "100", "200", "500", "1000"])
                .ConfigureAwait(ConfigureAwaitOptions.None);
            return;
        }

        if (_tipsCalculationRequest.TipSize is null)
        {
            _state = CurrentState.TipSizeRequested;
            await dependencies.TgBotClient.SendButtons(updateModel.ChatId, "Какой процент считаем?",
                ["7", "10", "13"])
                .ConfigureAwait(ConfigureAwaitOptions.None);
            return;
        }

        var result = TipsCalculator.Calculate(_tipsCalculationRequest.AccountSum.Value,
            _tipsCalculationRequest.Denomination.Value, _tipsCalculationRequest.TipSize.Value);

        await dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId, result.ToString())
            .ConfigureAwait(ConfigureAwaitOptions.None);
    }

    private async Task<bool> ProcessDenominationReceived(UpdateModel updateModel, TipsCalcDependencies dependencies)
    {
        var denominationForParsing = updateModel.CallbackQuery is null
            ? updateModel.Text
            : updateModel.CallbackQuery.Data;

        if (!BanknoteDenomination.TryParse(denominationForParsing, null, out var denominationParsed))
        {
            await dependencies.TgBotClient.SendTextMessageAsync(updateModel.ChatId, 
                    $"Не удалось считать минимальную купюру из \'{updateModel.Text}\'")
                .ConfigureAwait(ConfigureAwaitOptions.None);
            return true;
        }

        _tipsCalculationRequest.Denomination = denominationParsed;
        return false;
    }

    private async Task<bool> ProcessTipSizeReceived(UpdateModel updateModel, TipsCalcDependencies dependencies)
    {
        var tipsSize = updateModel.CallbackQuery is null
            ? updateModel.Text
            : updateModel.CallbackQuery.Data;
        tipsSize ??= "null";
        if (!int.TryParse(tipsSize, out var percentParsed))
        {
            await dependencies.TgBotClient.SendTextMessageAsync(
                updateModel.ChatId, $"Не удалось считать размер процентов из \'{tipsSize}\'");
            return true;
        }
        _tipsCalculationRequest.TipSize = percentParsed;
        return false;
    }

    private async Task<bool> ProcessSumReceived(UpdateModel updateModel, TipsCalcDependencies dependencies)
    {
        if (!decimal.TryParse(updateModel.Text, CultureInfo.InvariantCulture, out var sumParsed))
        {
            await dependencies.TgBotClient.SendTextMessageAsync(
                    updateModel.ChatId, $"Не удалось считать сумму из строки \'{updateModel.Text}\'")
                .ConfigureAwait(ConfigureAwaitOptions.None);
            return true;
        }

        _tipsCalculationRequest.AccountSum = sumParsed;
        return false;
    }

    private enum CurrentState : byte
    {
        Start,
        SumRequested,
        DenominationRequested,
        TipSizeRequested
    }
}

public class TipsCalcDependencies : IStateDependencies
{
    public static string DependencyKey => nameof(TipsCalcDependencies);

    public ITgBotClient TgBotClient { get; }
    public IStateManager StateManager { get; }

    public TipsCalcDependencies(ITgBotClient tgBotClient,
        IStateManager stateManager)
    {
        TgBotClient = tgBotClient;
        StateManager = stateManager;
    }
}