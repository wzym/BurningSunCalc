using AssistantBot.Interfaces;
using AssistantBot.Types;
using AssistantBot.Types.Dtos;
using System.Text.Json;

namespace AssistantBot.Api.AspHelpers;

public class UpdateRequestMappingMiddleware : IMiddleware
{
    internal const string UpdateModelItemKey = "UpdateModel";
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ILogger<UpdateRequestMappingMiddleware> _logger;
    private readonly IUpdateMessageParser<UpdateDto> _messageParser;

    public UpdateRequestMappingMiddleware(ILogger<UpdateRequestMappingMiddleware> logger,
        IUpdateMessageParser<UpdateDto> messageParser)
    {
        _logger = logger;
        _messageParser = messageParser;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var reader = new StreamReader(context.Request.Body);
        var requestBody = await reader.ReadToEndAsync();
        
        try
        {
            var updateReceived = JsonSerializer.Deserialize<UpdateDto>(requestBody, JsonSerializerOptions);
            if (updateReceived is null)
            {
                _logger.LogError("Unable to parse an update model from a {RawRequestBody}", requestBody);
                throw new AssistantBotException("Unable to parse an update model");
            }
            var updateModel = _messageParser.Parse(updateReceived);
            context.Items.Add(UpdateModelItemKey, updateModel);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to parse an update model from a {RawRequestBody}", requestBody);
            throw new AssistantBotException("Unable to parse an update model");
        }
        finally
        {
            reader.Dispose();
        }

        await next(context);
    }
}