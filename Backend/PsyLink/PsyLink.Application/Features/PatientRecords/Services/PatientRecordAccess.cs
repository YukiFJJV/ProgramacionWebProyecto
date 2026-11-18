using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.PatientRecords.Services;

internal static class PatientRecordAccess
{
    public static Guid RequirePsychologistTenant(ICurrentUserService currentUser)
    {
        if (currentUser.TenantId is not Guid tenantId ||
            currentUser.RoleName != RoleNames.Psychologist)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La operación requiere un psicólogo autenticado dentro de una empresa.");
        }

        return tenantId;
    }

    public static async Task<(PatientRecord Record, Guid TenantId)> GetAuthorizedRecordAsync(
        Guid recordId,
        IPatientRecordRepository recordRepository,
        IAppointmentRepository appointmentRepository,
        ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        var tenantId = RequirePsychologistTenant(currentUser);
        var record = await recordRepository.GetByRecordIdForTenantAsync(
                recordId,
                tenantId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el expediente {recordId}.");

        if (!await appointmentRepository.HasActiveCareRelationshipAsync(
                record.PatientId,
                currentUser.UserId,
                tenantId,
                cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "El psicólogo no mantiene una relación asistencial activa con este paciente.");
        }

        return (record, tenantId);
    }
}
