using FluentValidation;
using PsyLink.Application.Features.Users.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Users.Validators;

public sealed class GetUsersValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersValidator(IUserRepository userRepository)
    {
        RuleFor(query => query.Page).GreaterThan(0);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Search).MaximumLength(200);
        RuleFor(query => query.SortBy)
            .Must(sortBy => sortBy is not null &&
                new[] { "firstName", "lastName", "email", "role" }
                    .Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage("El campo de ordenamiento no está permitido.");
        RuleFor(query => query.SortDirection)
            .Must(direction => direction is not null &&
                (direction.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                 direction.Equals("desc", StringComparison.OrdinalIgnoreCase)))
            .WithMessage("La dirección de ordenamiento debe ser asc o desc.");
        RuleFor(query => query.RoleId)
            .MustAsync(async (roleId, cancellationToken) =>
            {
                if (!roleId.HasValue)
                {
                    return true;
                }

                var role = await userRepository.GetRoleByIdAsync(roleId.Value, cancellationToken);
                return role is not null && role.Name != RoleNames.SuperAdmin;
            })
            .WithMessage("El filtro debe ser un rol asignable distinto de SUPER_ADMIN.");
    }
}
