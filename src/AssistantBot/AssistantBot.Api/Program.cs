using AssistantBot.Api.AspHelpers;
using AssistantBot.Interfaces;
using AssistantBot.Logic;
using AssistantBot.Logic.Services;
using AssistantBot.Logic.StateMachine;
using AssistantBot.Logic.StateMachine.BurningSunStates;
using AssistantBot.Logic.StateMachine.Divination.SuffMiddleAge;
using AssistantBot.Logic.StateMachine.OtherStates;
using AssistantBot.Logic.StateMachine.SettingsStates;
using AssistantBot.Logic.StateMachine.TipsCalculator;
using AssistantBot.Types.Dtos;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();
builder.Configuration.AddConfiguration(config, true);

var botConfigSection = builder.Configuration.GetSection("BotConfiguration");

#region DI
builder.Services.AddProblemDetails()
    .AddExceptionHandler<CustomExceptionHandler>();

builder.Services
    .Configure<BotConfiguration>(botConfigSection)
    .AddScoped<UpdateModelHolder>()
    .AddTransient<UpdateRequestMappingMiddleware>()
    .AddTransient<UpdateRequestAuthMiddleware>()            
    .AddTransient<IUpdateMessageParser<UpdateDto>, TgBotUpdateParser>()            
    .AddSingleton<ITgBotSecretTokenProvider, TgBotSecretTokenProvider>()            
    .AddTransient<ITgUpdateHandler, TgUpdateHandler>()            
    .AddTransient<ITgBotWebHookConnector, TgBotWebHookConnector>()            
    .AddTransient<ITgBotClient, TgBotClient>()
    .AddTransient<ISuffMiddleageFortuneTeller, SuffMiddleageFortuneTeller>()
    .AddTransient<IBurningSunResponseGenerator, BurningSunResponseGenerator>()
    .AddTransient<IStateDependenciesResolver, StateDependenciesResolver>()
    .AddKeyedTransient<DefaultStateDependencies>(DefaultStateDependencies.DependencyKey)
    .AddKeyedTransient<BurningSunCalcDependencies>(BurningSunCalcDependencies.DependencyKey)
    .AddKeyedTransient<SunAngleChangeDependencies>(SunAngleChangeDependencies.DependencyKey)
    .AddKeyedTransient<NewSunPowerRespIsAwaitedDependencies>(NewSunPowerRespIsAwaitedDependencies.DependencyKey)
    .AddKeyedTransient<DivinationRequestedDependencies>(DivinationRequestedDependencies.DependencyKey)
    .AddKeyedTransient<NewCoordinatesSetupDependencies>(NewCoordinatesSetupDependencies.DependencyKey)
    .AddKeyedTransient<NewCoordinatesAwaitedDependencies>(NewCoordinatesAwaitedDependencies.DependencyKey)
    .AddKeyedTransient<SmthElseStateDependencies>(SmthElseStateDependencies.DependencyKey)
    .AddKeyedTransient<TipsCalcDependencies>(TipsCalcDependencies.DependencyKey)
    .AddSingleton<IIdentifierManager, IdentifierManager>()
    .AddSingleton<IStateManager, StateManager>()
    .AddSingleton<IChatSettingsStore, ChatSettingsStore>()
    .AddSingleton<ICommandsDispatcher, CommandsDispatcher>()
    .AddSerilog(s => s.WriteTo.Console().MinimumLevel.Information()
        .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
        .Destructure.ToMaximumDepth(6)
        .Destructure.ToMaximumStringLength(500))
    .AddHttpClient("tgwebhook").RemoveAllLoggers()
    .AddTypedClient<ITelegramBotClient>(httpClient =>            
        new TelegramBotClient(botConfigSection.Get<BotConfiguration>()!.BotToken, httpClient))
    .AddStandardResilienceHandler();
builder.Services.AddHostedService<InitService>()
    .AddHostedService<RefreshingSecretService>();
#endregion
var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogDebug("The app has been built");
#region Middlewares        
app.UseExceptionHandler()
    .UseHttpsRedirection()        
    .UseMiddleware<UpdateRequestAuthMiddleware>()
    .UseMiddleware<UpdateRequestMappingMiddleware>();
#endregion
app.MapPost("/bot/update",            
    async([FromServices] UpdateModelHolder updateModelHolder,    
    [FromServices] ITgUpdateHandler tgUpdateHandler) =>            
    {    
        await tgUpdateHandler.Handle(updateModelHolder.UpdateModel);            
        return TypedResults.Ok();            
    })
    .AddEndpointFilter<SenderFilter>()    
    .WithName("PostBotUpdate")            
    .Produces(StatusCodes.Status200OK)            
    .ProducesValidationProblem();

logger.LogDebug("The app is being started");
app.Run();