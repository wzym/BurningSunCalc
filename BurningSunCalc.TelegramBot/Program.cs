using BurningSunCalc;
using BurningSunCalc.AstroCalc;
using BurningSunCalc.Interfaces;
using BurningSunCalc.TelegramBot;
using BurningSunCalc.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;

var appConfiguration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();
var secrets = new BurningSunCalcSecrets { TelegramBotSecretToken = appConfiguration["AppSecrets:TelegramBotSecretToken"] };
var coordinates = appConfiguration.GetSection("Coordinates").Get<Coordinates>();
using var cts = new CancellationTokenSource();

var serviceCollection = new ServiceCollection();
serviceCollection
    .AddLogging()
    .AddTransient<ITgBotMessagesHandler, TgBotMessagesHandler>()
    .AddKeyedTransient<ISubHandler, BurningSunHandler>(BurningSunHandler.DependencyKey)
    .AddKeyedTransient<ISubHandler, SmthElseHandler>(SmthElseHandler.DependencyKey)
    .AddTransient<IAstroCalc, CustomAstroCalc>(_ => new CustomAstroCalc(coordinates))
    .AddHttpClient<ITelegramBotClient, TelegramBotClient>(httpClient => 
    {
        var result = new TelegramBotClient(secrets.TelegramBotSecretToken, httpClient: httpClient, cancellationToken: cts.Token);
        return result;
    })
    .AddStandardResilienceHandler();

var serviceProvider = serviceCollection.BuildServiceProvider();
var botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var handler = serviceProvider.GetRequiredService<ITgBotMessagesHandler>();
logger.LogDebug("Start of receiving");
await botClient.SetMyCommands(OwnCommands.Value);
botClient.StartReceiving(handler.Handle, HandleErrorAsync, new ReceiverOptions());

Console.ReadLine();
await cts.CancelAsync();

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine(exception.ToString());
    return Task.CompletedTask;
}