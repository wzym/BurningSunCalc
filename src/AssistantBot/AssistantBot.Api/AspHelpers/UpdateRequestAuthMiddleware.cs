using AssistantBot.Interfaces;
using System.Reflection.PortableExecutable;
using TgBotAbstractions;

namespace AssistantBot.AspHelpers;

internal class UpdateRequestAuthMiddleware : IMiddleware
{
    private readonly ILogger<UpdateRequestAuthMiddleware> _logger;
    private readonly ITgBotSecretTokenProvider _tgBotSecretTokenProvider;

    public UpdateRequestAuthMiddleware(ILogger<UpdateRequestAuthMiddleware> logger,
        ITgBotSecretTokenProvider tgBotSecretTokenProvider)
    {
        _logger = logger;
        _tgBotSecretTokenProvider = tgBotSecretTokenProvider;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Headers[Constants.TelegramBotSecretKeyHeader] != _tgBotSecretTokenProvider.Get)
        {
            using var reader = new StreamReader(context.Request.Body);
            var requestBody = await reader.ReadToEndAsync();
            var headers = context.Request.Headers;

            _logger.LogInformation("An update Request with wrong secret token received: {Body}, {@Headers}",
                requestBody, context.Request.Headers);
            await TypedResults.Ok().ExecuteAsync(context);

            return;
        }

        await next(context);
    }
}