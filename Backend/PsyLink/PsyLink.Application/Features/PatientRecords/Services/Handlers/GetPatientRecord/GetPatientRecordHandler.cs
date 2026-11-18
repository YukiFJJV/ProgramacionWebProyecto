using MediatR;
using PsyLink.Application.Features.PatientRecords.DTOs;
using PsyLink.Application.Features.PatientRecords.Services;
using PsyLink.Application.Features.PatientRecords.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Features.PatientRecords.Services.Handlers.GetPatientRecord;

public sealed class GetPatientRecordHandler(
    IPatientRecordRepository recordRepository,
    IAppointmentRepository appointmentRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetPatientRecordQuery, RecordDetailResponseDto>
{
    public async Task<RecordDetailResponseDto> Handle(
        GetPatientRecordQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = PatientRecordAccess.RequirePsychologistTenant(currentUser);
        if (!await appointmentRepository.HasActiveCareRelationshipAsync(
                request.PatientId,
                currentUser.UserId,
                tenantId,
                cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "El psicólogo no mantiene una relación asistencial activa con este paciente.");
        }

        var record = await recordRepository.GetByPatientIdWithDetailsAsync(
                request.PatientId,
                tenantId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró expediente para el paciente {request.PatientId}.");

        var appointments = await appointmentRepository.GetByPatientAndTenantAsync(
            request.PatientId,
            tenantId,
            cancellationToken);
        var lastAttendedAt = appointments
            .Where(appointment => appointment.Status == AppointmentStatus.Completed)
            .Select(appointment => (DateTime?)appointment.DateAt.ToDateTime(appointment.EndsAt))
            .Max();
        var effectiveRiskScore = RiskPolicy.EffectiveRisk(
            record.RiskScore,
            lastAttendedAt,
            DateTime.UtcNow);

        return PatientRecordDtoMapper.ToDetail(record, appointments, effectiveRiskScore);
    }
}
