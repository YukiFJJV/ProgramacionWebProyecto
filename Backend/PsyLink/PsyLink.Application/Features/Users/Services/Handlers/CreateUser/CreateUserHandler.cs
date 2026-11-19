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
    ICurrentUserService currentUser,
    IPublicRegistrationSettings publicRegistrationSettings,
    IRoleSettings roleSettings)
    : IRequestHandler<CreateUserCommand, UserResponseDto>
{
    public async Task<UserResponseDto> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByEmailAsync(request.User.Email, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("Ya existe una cuenta con ese correo electrónico.");
        }

        var isCompanyAdmin = currentUser.IsAuthenticated
            && currentUser.RoleName == RoleNames.Admin
            && currentUser.TenantId is not null;

        Tenant tenant;
        Role role;

        if (isCompanyAdmin)
        {
            var tenantId = currentUser.TenantId!.Value;
            tenant = await tenantRepository.GetByIdAsync(tenantId, cancellationToken)
                ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró la empresa {tenantId}.");

            if (request.User.RoleId is null)
            {
                role = await userRepository.GetRoleByIdAsync(roleSettings.PatientRoleId, cancellationToken)
                    ?? throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El rol PATIENT configurado no existe.");
            }
            else
            {
                role = await userRepository.GetRoleByIdAsync(request.User.RoleId.Value, cancellationToken)
                    ?? throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("Debe asignarse un rol válido distinto de SUPER_ADMIN.");

                if (role.Name == RoleNames.SuperAdmin)
                {
                    throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("Debe asignarse un rol válido distinto de SUPER_ADMIN.");
                }
            }
        }
        else
        {
            var configuredTenantId = publicRegistrationSettings.DefaultTenantId;
            if (configuredTenantId is not null)
            {
                tenant = await tenantRepository.GetByIdAsync(configuredTenantId.Value, cancellationToken)
                    ?? throw new PsyLink.Application.Common.Exceptions.ConflictException(
                        "La empresa configurada para el registro público no existe.");
            }
            else
            {
                tenant = await tenantRepository.GetFirstActiveAsync(cancellationToken)
                    ?? throw new PsyLink.Application.Common.Exceptions.ConflictException(
                        "No hay una empresa activa disponible para el registro público.");
            }

            role = await userRepository.GetRoleByIdAsync(roleSettings.PatientRoleId, cancellationToken)
                ?? throw new PsyLink.Application.Common.Exceptions.ConflictException(
                    "El rol PATIENT configurado no existe en la base de datos.");
        }

        if (!tenant.Active)
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("No se pueden registrar usuarios en una empresa inactiva.");
        }

        var phoneNumbers = new List<string>();
        if (!string.IsNullOrWhiteSpace(request.User.PhoneNumber))
        {
            phoneNumbers.Add(request.User.PhoneNumber.Trim());
        }
        if (request.User.PhoneNumbers is not null)
        {
            phoneNumbers.AddRange(request.User.PhoneNumbers
                .Where(number => !string.IsNullOrWhiteSpace(number))
                .Select(number => number.Trim()));
        }

        var phones = phoneNumbers
            .Distinct(StringComparer.Ordinal)
            .Select(number => new PhoneNumber
            {
                PhoneId = Guid.NewGuid(),
                Number = number
            })
            .ToList();
        var primaryPhone = phones.FirstOrDefault();
        var user = new User
        {
            UserId = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            PhoneId = primaryPhone?.PhoneId,
            Phone = primaryPhone,
            Phones = phones,
            RoleId = role.RoleId,
            Role = role,
            FirstName = request.User.FirstName,
            LastName = request.User.LastName,
            Email = request.User.Email,
            PasswordHash = passwordHasher.Hash(request.User.Password),
            Active = true
        };

        await userRepository.AddAsync(user, cancellationToken);
        return PsyLink.Application.Features.Users.Shared.UserDtoMapper.ToResponse(user);
    }

}
