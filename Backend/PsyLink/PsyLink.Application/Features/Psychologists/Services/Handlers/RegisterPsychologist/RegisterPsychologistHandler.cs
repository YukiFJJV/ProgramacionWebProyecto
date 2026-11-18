using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Psychologists.DTOs;
using PsyLink.Application.Features.Psychologists.Services.Commands;
using PsyLink.Application.Features.Specialties.DTOs;
using PsyLink.Application.Features.Users.Services.Handlers.CreateUser;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Psychologists.Services.Handlers.RegisterPsychologist;

public sealed class RegisterPsychologistHandler(
    IUserRepository userRepository,
    ITenantRepository tenantRepository,
    ISpecialtyRepository specialtyRepository,
    IPsychologistRepository psychologistRepository,
    IPasswordHasher passwordHasher,
    ICurrentUserService currentUser)
    : IRequestHandler<RegisterPsychologistCommand, PsychologistDetailResponseDto>
{
    public async Task<PsychologistDetailResponseDto> Handle(
        RegisterPsychologistCommand request,
        CancellationToken cancellationToken)
    {
        EnsureCompanyAdmin();

        var userRequest = request.Psychologist.User;
        if (await userRepository.ExistsByEmailAsync(userRequest.Email, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("Ya existe una cuenta con ese correo electrónico.");
        }

        if (await psychologistRepository.ExistsByLicenseNumberAsync(
                request.Psychologist.LicenseNumber,
                cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El número de licencia ya está registrado.");
        }

        var tenantId = currentUser.TenantId
            ?? throw new PsyLink.Application.Common.Exceptions.ConflictException("El usuario autenticado no tiene una empresa asignada.");
        var tenant = await tenantRepository.GetByIdAsync(tenantId, cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró la empresa {tenantId}.");
        if (!tenant.Active)
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("No se pueden registrar psicólogos en una empresa inactiva.");
        }

        var specialty = await specialtyRepository.GetByIdAsync(
                request.Psychologist.SpecialtyId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException(
                $"No se encontró la especialidad {request.Psychologist.SpecialtyId}.");
        var role = await userRepository.GetRoleByNameAsync(RoleNames.Psychologist, cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.ConflictException("No está configurado el rol PSYCHOLOGIST.");

        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(userRequest.PhoneNumber))
        {
            phone = new PhoneNumber
            {
                PhoneId = Guid.NewGuid(),
                Number = userRequest.PhoneNumber
            };
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            PhoneId = phone?.PhoneId,
            Phone = phone,
            RoleId = role.RoleId,
            Role = role,
            FirstName = userRequest.FirstName,
            LastName = userRequest.LastName,
            Email = userRequest.Email,
            PasswordHash = passwordHasher.Hash(userRequest.Password),
            Active = true
        };
        var psychologist = new Psychologist
        {
            PsychologistId = Guid.NewGuid(),
            UserId = user.UserId,
            TenantId = tenant.TenantId,
            SpecialtyId = specialty.SpecialtyId,
            AdminId = currentUser.UserId,
            LicenseNumber = request.Psychologist.LicenseNumber,
            User = user,
            Tenant = tenant,
            Specialty = specialty
        };

        await psychologistRepository.AddWithUserAsync(psychologist, cancellationToken);

        return new PsychologistDetailResponseDto
        {
            PsychologistId = psychologist.PsychologistId,
            User = CreateUserHandler.ToResponse(user),
            Specialty = new SpecialtyResponseDto
            {
                SpecialtyId = specialty.SpecialtyId,
                SpecialtyName = specialty.SpecialtyName
            },
            LicenseNumber = psychologist.LicenseNumber
        };
    }

    private void EnsureCompanyAdmin()
    {
        if (currentUser.RoleName != RoleNames.Admin || currentUser.TenantId is null)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere un administrador de empresa autenticado.");
        }
    }
}
