using AssistantBot.Types;
using Microsoft.AspNetCore.Diagnostics;

namespace AssistantBot.Api.AspHelpers;

internal class CustomExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger;

    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is AssistantBotException abe)
        {
            _logger.LogError(abe, "Own exception was caught");
        }
        else
        {
            _logger.LogError(exception, "Caught an unknown exception");
        }

        await TypedResults.Ok().ExecuteAsync(httpContext);
        return true;
    }
}