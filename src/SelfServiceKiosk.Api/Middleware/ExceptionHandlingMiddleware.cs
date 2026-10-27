using Microsoft.AspNetCore.Mvc;
using SelfServiceKiosk.Api.Exceptions;

namespace SelfServiceKiosk.Api.Middleware;

/// <summary>
/// Global exception handling (NFR-11). Every error leaves the API in the same shape -
/// an RFC 7807 "problem details" JSON body - with the correct HTTP status code.
/// Stack traces and technical details are never sent to the client (NFR-12).
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception) when (!context.Response.HasStarted)
        {
            await WriteProblemAsync(context, exception);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        // Map each known exception to a status code and a friendly message.
        // When you add a new exception type, add one line here.
        var (statusCode, title) = exception switch
        {
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, exception.Message),
            AccountDeactivatedException => (StatusCodes.Status403Forbidden, exception.Message),
            EmailAlreadyExistsException => (StatusCodes.Status409Conflict, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Something went wrong. Please try again."),
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            // Logged with the detail needed to investigate (NFR-13); the client only sees the friendly title.
            _logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}.",
                context.Request.Method,
                context.Request.Path.Value);
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Instance = context.Request.Path.Value,
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
    }
}
