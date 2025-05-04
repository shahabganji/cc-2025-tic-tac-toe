using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace TicTacToe;

public class TicTacToeGlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IWebHostEnvironment env,
    ILogger<TicTacToeGlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        return exception is InvalidOperationException
            ? await TreatExceptionsAs400BadRequest(httpContext, exception)
            : await TreatExceptionsAs500InternalServerError(httpContext, exception);
    }

    private async Task<bool> TreatExceptionsAs400BadRequest(HttpContext httpContext, Exception exception)
    {
        var problemDetail = new ProblemDetails
        {
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400",
            Title = exception.Message,
            Detail = env.IsDevelopment()
                ? exception.StackTrace
                : string.Empty,
        };

        problemDetail.Extensions.TryAdd("requestId", httpContext.TraceIdentifier);

        var activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        problemDetail.Extensions.TryAdd("traceId", activity?.Id ?? Guid.NewGuid().ToString());

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetail,
        });
    }

    private async Task<bool> TreatExceptionsAs500InternalServerError(HttpContext httpContext, Exception exception)
    {
        const string errorMessage =
            "Some error occurred while processing your request. please use the traceId to contact support";

        var problemDetail = new ProblemDetails
        {
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.com/500",
            Title = env.IsDevelopment()
                ? exception.Message
                : errorMessage,
            Detail = env.IsDevelopment()
                ? exception.StackTrace
                : errorMessage,
        };

        problemDetail.Extensions.TryAdd("requestId", httpContext.TraceIdentifier);

        var activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        var traceId = activity?.Id ?? Guid.NewGuid().ToString();
        problemDetail.Extensions.TryAdd("traceId", traceId);

        logger.LogError(exception,
            "An unhandled exception has occurred while executing the request: {ExceptionMessage} {TraceId}",
            exception.Message, traceId);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetail,
        });
    }
}
