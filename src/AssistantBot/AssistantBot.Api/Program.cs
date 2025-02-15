using AssistantBot.Api.AspHelpers;
using AssistantBot.AspHelpers;
using AssistantBot.Interfaces;
using AssistantBot.Logic;
using AssistantBot.Logic.Services;
using AssistantBot.Logic.StateMachine.BurningSunStates;
using AssistantBot.Types;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

var botConfigSection = builder.Configuration.GetSection("BotConfiguration");

#region DI
builder.Services
    .Configure<BotConfiguration>(botConfigSection)            
    .AddTransient<UpdateRequestMappingMiddleware>()            
    .AddTransient<UpdateRequestAuthMiddleware>()            
    .AddTransient<IUpdateMessageParser<Update>, TgBotUpdateParser>()            
    .AddSingleton<ITgBotSecretTokenProvider, TgBotSecretTokenProvider>()            
    .AddTransient<ITgUpdateHandler, TgUpdateHandler>()            
    .AddTransient<ITgBotWebHookConnector, TgBotWebHookConnector>()            
    .AddTransient<ITgBotClient, TgBotClient>()
    .AddTransient<IBurningSunResponseGenerator, BurningSunResponseGenerator>()
    .AddTransient<IStateDependenciesResolver, StateDependenciesResolver>()
    .AddTransient<BurningSunCalcDependencies>()
    .AddSingleton<IStateManager, StateManager>()
    .AddSingleton<IChatSettingsStore, ChatSettingsStore>()
    .AddSingleton<ICommandsDispatcher, CommandsDispatcher>()
    .AddSingleton<PowerSensitivitySettingsManager>()
    .ConfigureTelegramBotMvc()            
    .AddSerilog(s => s.WriteTo.Console().MinimumLevel.Debug())
    .AddHttpClient("tgwebhook").RemoveAllLoggers()            
    .AddTypedClient<ITelegramBotClient>(httpClient =>            
        new TelegramBotClient(botConfigSection.Get<BotConfiguration>()!.BotToken, httpClient))
    .AddStandardResilienceHandler();
builder.Services.AddHostedService<InitService>();
#endregion
var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogDebug("The app has been built");
#region Middlewares        
app.UseHttpsRedirection()        
    .UseMiddleware<UpdateRequestAuthMiddleware>()        
    .UseMiddleware<UpdateRequestMappingMiddleware>();        
#endregion        
app.MapPost("/bot/update",        
    async(HttpContext httpContext,
    [FromServices] ITgUpdateHandler tgUpdateHandler) =>        
    {
        var updateModel = (UpdateModel)httpContext.Items[UpdateRequestMappingMiddleware.UpdateModelItemKey]!;
        await tgUpdateHandler.Handle(updateModel);        
        return TypedResults.Ok();        
    })        
    .WithName("PostBotUpdate")        
    .Produces(StatusCodes.Status200OK)        
    .ProducesValidationProblem(StatusCodes.Status400BadRequest);

logger.LogDebug("The app is being started");
app.Run();