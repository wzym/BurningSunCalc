using AssistantBot.Interfaces;
using System.Text.Json;
using Telegram.Bot.Types;

namespace AssistantBot.AspHelpers;

internal class UpdateRequestMappingMiddleware : IMiddleware
{
    internal const string UpdateModelItemKey = "UpdateModel";

    private readonly ILogger<UpdateRequestMappingMiddleware> _logger;
    private readonly IUpdateMessageParser<Update> _messageParser;

    public UpdateRequestMappingMiddleware(ILogger<UpdateRequestMappingMiddleware> logger,
        IUpdateMessageParser<Update> messageParser)
    {
        _logger = logger;
        _messageParser = messageParser;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var reader = new StreamReader(context.Request.Body);
        try
        {
            var requestBody = await reader.ReadToEndAsync();
            var updateReceived = JsonSerializer.Deserialize<Update>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (updateReceived is null)
            {
                _logger.LogWarning("Unable to parse an update model from {RequestBodyAsString}", requestBody);
                await TypedResults.Ok().ExecuteAsync(context);
                return;
            }
            var updateModel = _messageParser.Parse(updateReceived);
            context.Items.Add(UpdateModelItemKey, updateModel);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to parse an update model");
        }
        finally
        {
            reader.Dispose();
        }
        

        await next(context);
    }
}