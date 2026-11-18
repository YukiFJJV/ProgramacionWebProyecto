using MediatR;
using PsyLink.Application.Common.Authorization;
using PsyLink.Application.Common.Exceptions;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse>(
    ICurrentUserService currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var attributes = request.GetType().GetCustomAttributes(
            typeof(AuthorizeAttribute),
            inherit: true)
            .Cast<AuthorizeAttribute>()
            .ToArray();

        if (attributes.Length > 0)
        {
            if (!currentUser.IsAuthenticated)
            {
                throw new ForbiddenException("Se requiere un usuario autenticado.");
            }

            var allowedRoles = attributes
                .SelectMany(attribute => attribute.Roles)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (allowedRoles.Length > 0 &&
                !allowedRoles.Contains(
                    currentUser.RoleName,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new ForbiddenException("El usuario no tiene permiso para ejecutar esta operación.");
            }
        }

        return await next();
    }
}
