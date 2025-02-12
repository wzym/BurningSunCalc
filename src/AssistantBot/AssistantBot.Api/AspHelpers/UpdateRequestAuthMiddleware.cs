using AssistantBot.Interfaces;
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
            _logger.LogInformation("An update {@request} with wrong secret token received", context.Request);
            await TypedResults.Forbid().ExecuteAsync(context);

            return;
        }

        await next(context);
    }
}