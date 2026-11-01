using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FullStackProject.Application.Exceptions;


namespace FullStackProject.API.ExceptionHandling;

public sealed class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        RequestDelegate next,
        ILogger<GlobalExceptionHandler> logger)
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
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            var statusCode = exception switch
            {
                UnauthenticatedUserException =>
                    StatusCodes.Status401Unauthorized,

                UnauthorizedAccessException =>
                    StatusCodes.Status403Forbidden,

                _ => StatusCodes.Status500InternalServerError
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning(
                    "Authorization failure for {Method} {Path}. StatusCode: {StatusCode}. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    statusCode,
                    context.TraceIdentifier);
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";
            context.Response.Headers["X-Trace-Id"] = context.TraceIdentifier;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode switch
                {
                    StatusCodes.Status401Unauthorized => "Authentication required.",
                    StatusCodes.Status403Forbidden => "Access denied.",
                    _ => "An unexpected error occurred."
                },
                Detail = statusCode switch
                {
                    StatusCodes.Status401Unauthorized =>
                        "The authenticated user could not be identified.",
                    StatusCodes.Status403Forbidden =>
                        "You do not have permission to perform this operation.",
                    _ =>
                        "The server encountered an unexpected error. Please try again later."
                },
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(
                    problem,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)));
        }
    }
}
