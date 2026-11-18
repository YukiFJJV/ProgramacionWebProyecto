using MediatR;
using PsyLink.Application.Common.Interfaces;
using PsyLink.Application.Common.Messaging;
using PsyLink.Application.Common.Exceptions;
using PsyLink.Domain.Common;

namespace PsyLink.Application.Features.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICommand<TResponse>)
        {
            return await next();
        }

        try
        {
            var response = await next();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return response;
        }
        catch (DomainException exception)
        {
            throw new DomainApplicationException(exception.Message);
        }
    }
}
