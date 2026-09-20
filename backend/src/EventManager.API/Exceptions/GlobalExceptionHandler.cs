using EventManager.Application.Common.Exceptions;
using EventManager.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Exceptions;

public sealed class GlobalExceptionHandler
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler>
        _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger
    ) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var statusCode = 
            exception switch
            {
                ValidationException => 
                    StatusCodes.Status400BadRequest,

                DomainException => 
                    StatusCodes.Status400BadRequest,

                ConflictException => 
                    StatusCodes.Status409Conflict,

                NotFoundException => 
                    StatusCodes.Status404NotFound,

                UnauthorizedException => 
                    StatusCodes.Status401Unauthorized,

                _ => 
                    StatusCodes.Status500InternalServerError
            };

        if (statusCode ==
            StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred."
            );
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = 
                statusCode == 
                StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message,
            
            Instance = 
                httpContext.Request.Path
        };

        httpContext.Response.StatusCode =
            statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            cancellationToken
        );

        return true;
    }

    private static string GetTitle(
        int statusCode
    ) => 
        statusCode switch
        {
            400 => "Bad request",
            401 => "Unauthorized",
            404 => "Not found",
            409 => "Conflict",
            _ => "Internal Server Error"
        };
}