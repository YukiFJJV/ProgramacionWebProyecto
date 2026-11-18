using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Common.Exceptions;

namespace PsyLink.API.Middlewares;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            var isInvalidLogin = httpContext.Request.Path.Value?.EndsWith(
                    "/auth/login",
                    StringComparison.OrdinalIgnoreCase) == true &&
                validationException.Errors.Any() &&
                validationException.Errors.All(error =>
                    error.ErrorMessage == "Correo electrónico o contraseña inválidos.");
            if (isInvalidLogin)
            {
                logger.LogWarning(exception, "Login credentials were rejected.");
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = "Unauthorized",
                        Detail = "Correo electrónico o contraseña inválidos.",
                        Instance = httpContext.Request.Path
                    },
                    cancellationToken);
                return true;
            }

            var validationErrors = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).Distinct().ToArray());
            logger.LogWarning(exception, "Request validation failed.");
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(
                new ValidationProblemDetails(validationErrors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred.",
                    Instance = httpContext.Request.Path
                },
                cancellationToken);
            return true;
        }

        var (statusCode, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden"),
            UnauthenticatedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            BusinessRuleException => (StatusCodes.Status400BadRequest, "Bad Request"),
            DomainApplicationException => (StatusCodes.Status422UnprocessableEntity, "Domain Rule Violation"),
            _ => (0, string.Empty)
        };

        if (statusCode == 0)
        {
            return false;
        }

        logger.LogWarning(exception, "Request failed with status {StatusCode}.", statusCode);
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            },
            cancellationToken);
        return true;
    }
}
