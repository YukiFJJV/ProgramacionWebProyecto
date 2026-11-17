using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Patients.Services.Commands;
using PsyLink.Application.Features.Users.Services.Handlers.CreateUser;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Patients.Services.Handlers.RegisterPatient;

public sealed class RegisterPatientHandler(
    IUserRepository userRepository,
    ITenantRepository tenantRepository,
    IPatientRepository patientRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<RegisterPatientCommand, PatientResponseDto>
{
    public async Task<PatientResponseDto> Handle(
        RegisterPatientCommand request,
        CancellationToken cancellationToken)
    {
        EnsureCompanyAdmin();

        var user = await userRepository.GetByIdAsync(request.Patient.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró la cuenta {request.Patient.UserId}.");
        var role = await userRepository.GetRoleByIdAsync(user.RoleId, cancellationToken)
            ?? throw new InvalidOperationException("La cuenta vinculada no tiene un rol válido.");
        user.Role = role;

        var tenantId = currentUser.TenantId
            ?? throw new InvalidOperationException("El usuario autenticado no tiene una empresa asignada.");
        var tenant = await tenantRepository.GetByIdAsync(tenantId, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró la empresa {tenantId}.");

        if (!tenant.Active)
        {
            throw new ValidationException("No se pueden registrar pacientes en una empresa inactiva.");
        }

        if (!user.Active)
        {
            throw new ValidationException("La cuenta de usuario está inactiva.");
        }

        if (user.TenantId != tenant.TenantId || role.Name != RoleNames.Patient)
        {
            throw new ValidationException("La cuenta debe pertenecer a la empresa y tener el rol PATIENT.");
        }

        if (await patientRepository.ExistsByUserIdAsync(user.UserId, cancellationToken))
        {
            throw new ValidationException("La cuenta ya tiene un perfil de paciente registrado.");
        }

        if (request.Patient.BirthDate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new ValidationException("La fecha de nacimiento no puede ser futura.");
        }

        var patient = new Patient
        {
            PatientId = Guid.NewGuid(),
            UserId = user.UserId,
            TenantId = tenant.TenantId,
            BirthDate = request.Patient.BirthDate,
            Description = request.Patient.Description,
            User = user,
            Tenant = tenant
        };

        await patientRepository.AddAsync(patient, cancellationToken);

        return new PatientResponseDto
        {
            PatientId = patient.PatientId,
            User = CreateUserHandler.ToResponse(user),
            BirthDate = patient.BirthDate,
            Description = patient.Description,
            Age = patient.AgeOn(DateOnly.FromDateTime(DateTime.UtcNow))
        };
    }

    private void EnsureCompanyAdmin()
    {
        if (currentUser.RoleName != RoleNames.Admin || currentUser.TenantId is null)
        {
            throw new UnauthorizedAccessException("La operación requiere un administrador de empresa autenticado.");
        }
    }
}
