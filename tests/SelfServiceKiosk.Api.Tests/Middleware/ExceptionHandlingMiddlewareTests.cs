using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using SelfServiceKiosk.Api.Exceptions;
using SelfServiceKiosk.Api.Middleware;

namespace SelfServiceKiosk.Api.Tests.Middleware;

/// <summary>Unit tests for the global error handling. A fake HTTP context is used, so nothing real starts.</summary>
public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_UnhandledException_Returns500WithGenericMessageAndNoDetails()
    {
        var context = CreateContext();
        var sut = CreateMiddleware(_ => throw new InvalidOperationException("secret detail for user@kiosk.test"));

        await sut.InvokeAsync(context);

        var body = await ReadBodyAsync(context);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.StartsWith("application/problem+json", context.Response.ContentType ?? string.Empty);
        Assert.Contains("Something went wrong", body);
        Assert.DoesNotContain("secret detail", body);
        Assert.DoesNotContain("user@kiosk.test", body);
        Assert.DoesNotContain("InvalidOperationException", body);
        Assert.DoesNotContain("StackTrace", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InvokeAsync_InvalidCredentials_Returns401()
    {
        var context = CreateContext();
        var sut = CreateMiddleware(_ => throw new InvalidCredentialsException());

        await sut.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_AccountDeactivated_Returns403()
    {
        var context = CreateContext();
        var sut = CreateMiddleware(_ => throw new AccountDeactivatedException());

        await sut.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_EmailAlreadyExists_Returns409()
    {
        var context = CreateContext();
        var sut = CreateMiddleware(_ => throw new EmailAlreadyExistsException());

        await sut.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
    }

    private static ExceptionHandlingMiddleware CreateMiddleware(RequestDelegate next)
    {
        return new ExceptionHandlingMiddleware(next, NullLogger<ExceptionHandlingMiddleware>.Instance);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadBodyAsync(DefaultHttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }
}
