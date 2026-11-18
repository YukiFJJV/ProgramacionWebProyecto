using MediatR;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Patients.Services.Handlers.GetPatients;
using PsyLink.Application.Features.Patients.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Patients.Services.Handlers.GetPatientById;

public sealed class GetPatientByIdHandler(
    IPatientRepository patientRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetPatientByIdQuery, PatientResponseDto>
{
    public async Task<PatientResponseDto> Handle(
        GetPatientByIdQuery request,
        CancellationToken cancellationToken)
    {
        var patient = currentUser.RoleName switch
        {
            RoleNames.Admin when currentUser.TenantId is Guid tenantId =>
                await patientRepository.GetByIdForTenantAsync(
                    request.PatientId,
                    tenantId,
                    cancellationToken),
            RoleNames.Psychologist =>
                await GetForPsychologistAsync(request.PatientId, cancellationToken),
            _ => throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La operación requiere un administrador de empresa o un psicólogo autenticado.")
        };

        if (patient is null)
        {
            throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el paciente {request.PatientId}.");
        }

        return GetPatientsHandler.ToResponse(patient);
    }

    private async Task<PsyLink.Domain.Clinical.Patient?> GetForPsychologistAsync(
        Guid patientId,
        CancellationToken cancellationToken)
    {
        var scope = PatientAccess.EnsurePsychologist(currentUser);
        return await patientRepository.GetByIdForPsychologistAsync(
            patientId,
            scope.TenantId,
            scope.UserId,
            cancellationToken);
    }
}
