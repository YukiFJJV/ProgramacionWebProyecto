using MediatR;
using Microsoft.Extensions.Logging;

namespace PsyLink.Application.Features.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Executing {RequestName}", typeof(TRequest).Name);
        var response = await next();
        logger.LogInformation("Executed {RequestName}", typeof(TRequest).Name);
        return response;
    }
}
