using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.Users.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Users.Services.Handlers.CreateUser;

public sealed class CreateUserHandler(
    IUserRepository userRepository,
    ITenantRepository tenantRepository,
    IPasswordHasher passwordHasher,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateUserCommand, UserResponseDto>
{
    public async Task<UserResponseDto> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        EnsureCompanyAdmin();

        if (await userRepository.ExistsByEmailAsync(request.User.Email, cancellationToken))
        {
            throw new ValidationException("Ya existe una cuenta con ese correo electrónico.");
        }

        var tenantId = currentUser.TenantId
            ?? throw new InvalidOperationException("El usuario autenticado no tiene una empresa asignada.");
        var tenant = await tenantRepository.GetByIdAsync(tenantId, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró la empresa {tenantId}.");
        if (!tenant.Active)
        {
            throw new ValidationException("No se pueden registrar usuarios en una empresa inactiva.");
        }

        var role = await userRepository.GetRoleByIdAsync(request.User.RoleId, cancellationToken);
        if (role is null || role.Name == RoleNames.SuperAdmin)
        {
            throw new ValidationException("Debe asignarse un rol válido distinto de SUPER_ADMIN.");
        }

        var phone = new PhoneNumber
        {
            PhoneId = Guid.NewGuid(),
            Number = request.User.PhoneNumber
                ?? throw new ValidationException("El teléfono es obligatorio.")
        };
        var user = new User
        {
            UserId = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            PhoneId = phone.PhoneId,
            Phone = phone,
            RoleId = role.RoleId,
            Role = role,
            FirstName = request.User.FirstName,
            LastName = request.User.LastName,
            Email = request.User.Email,
            PasswordHash = passwordHasher.Hash(request.User.Password),
            Active = true
        };

        await userRepository.AddAsync(user, cancellationToken);
        return ToResponse(user);
    }

    private void EnsureCompanyAdmin()
    {
        if (currentUser.RoleName != RoleNames.Admin || currentUser.TenantId is null)
        {
            throw new UnauthorizedAccessException("La operación requiere un administrador de empresa autenticado.");
        }
    }

    internal static UserResponseDto ToResponse(User user)
    {
        if (user.TenantId is null)
        {
            throw new InvalidOperationException("La cuenta registrada no tiene una empresa asignada.");
        }

        return new UserResponseDto
        {
            UserId = user.UserId,
            TenantId = user.TenantId.Value,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone is null
                ? null
                : new PhoneNumberResponseDto
                {
                    PhoneId = user.Phone.PhoneId,
                    Number = user.Phone.Number
                },
            Role = new RoleResponseDto
            {
                RoleId = user.Role.RoleId,
                Name = user.Role.Name
            }
        };
    }
}
