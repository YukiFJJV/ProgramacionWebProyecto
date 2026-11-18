using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Diagnoses.Services.Commands;
using PsyLink.Application.Features.Users.Services.Handlers.CreateUser;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Patients.Services.Handlers.RegisterPatient;

public sealed class RegisterPatientHandler(
    IUserRepository userRepository,
    ITenantRepository tenantRepository,
    IPatientRepository patientRepository,
    IPasswordHasher passwordHasher,
    ICurrentUserService currentUser)
    : IRequestHandler<RegisterPatientCommand, PatientResponseDto>
{
    public async Task<PatientResponseDto> Handle(
        RegisterPatientCommand request,
        CancellationToken cancellationToken)
    {
        EnsureCompanyAdmin();

        var tenantId = currentUser.TenantId
            ?? throw new PsyLink.Application.Common.Exceptions.ConflictException("El usuario autenticado no tiene una empresa asignada.");
        var tenant = await tenantRepository.GetByIdAsync(tenantId, cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró la empresa {tenantId}.");

        if (!tenant.Active)
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("No se pueden registrar pacientes en una empresa inactiva.");
        }

        if (await userRepository.ExistsByEmailAsync(request.Patient.User.Email, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("Ya existe una cuenta con ese correo electrónico.");
        }

        if (request.Patient.BirthDate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("La fecha de nacimiento no puede ser futura.");
        }

        var role = await userRepository.GetRoleByNameAsync(RoleNames.Patient, cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.ConflictException("No está configurado el rol PATIENT.");
        var phone = string.IsNullOrWhiteSpace(request.Patient.User.PhoneNumber)
            ? null
            : new PhoneNumber
            {
                PhoneId = Guid.NewGuid(),
                Number = request.Patient.User.PhoneNumber
            };
        var user = new User
        {
            UserId = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            PhoneId = phone?.PhoneId,
            Phone = phone,
            RoleId = role.RoleId,
            FirstName = request.Patient.User.FirstName,
            LastName = request.Patient.User.LastName,
            Email = request.Patient.User.Email,
            PasswordHash = passwordHasher.Hash(request.Patient.User.Password),
            Active = true
        };
        var patient = new Patient
        {
            PatientId = Guid.NewGuid(),
            UserId = user.UserId,
            TenantId = tenant.TenantId,
            BirthDate = request.Patient.BirthDate,
            Description = request.Patient.Description,
            User = user
        };

        await patientRepository.AddWithUserAsync(patient, user, cancellationToken);

        user.Role = role;
        return new PatientResponseDto
        {
            PatientId = patient.PatientId,
            User = PsyLink.Application.Features.Users.Shared.UserDtoMapper.ToResponse(user),
            BirthDate = patient.BirthDate,
            Description = patient.Description,
            Age = patient.AgeOn(DateOnly.FromDateTime(DateTime.UtcNow))
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
