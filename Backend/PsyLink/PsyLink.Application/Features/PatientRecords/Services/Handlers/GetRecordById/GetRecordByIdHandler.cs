using MediatR;
using PsyLink.Application.Features.PatientRecords.DTOs;
using PsyLink.Application.Features.PatientRecords.Services;
using PsyLink.Application.Features.PatientRecords.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Features.PatientRecords.Services.Handlers.GetRecordById;

public sealed class GetRecordByIdHandler(
    IPatientRecordRepository recordRepository,
    IAppointmentRepository appointmentRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetRecordByIdQuery, RecordDetailResponseDto>
{
    public async Task<RecordDetailResponseDto> Handle(
        GetRecordByIdQuery request,
        CancellationToken cancellationToken)
    {
        var (record, tenantId) = await PatientRecordAccess.GetAuthorizedRecordAsync(
            request.RecordId,
            recordRepository,
            appointmentRepository,
            currentUser,
            cancellationToken);

        var detailedRecord = await recordRepository.GetByPatientIdWithDetailsAsync(
                record.PatientId,
                tenantId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException(
                $"No se encontró expediente para el paciente {record.PatientId}.");
        var appointments = await appointmentRepository.GetByPatientAndTenantAsync(
            record.PatientId,
            tenantId,
            cancellationToken);
        var lastAttendedAt = appointments
            .Where(appointment => appointment.Status == AppointmentStatus.Completed)
            .Select(appointment => (DateTime?)appointment.DateAt.ToDateTime(appointment.EndsAt))
            .Max();
        var effectiveRiskScore = RiskPolicy.EffectiveRisk(
            detailedRecord.RiskScore,
            lastAttendedAt,
            DateTime.UtcNow);

        return PatientRecordDtoMapper.ToDetail(
            detailedRecord,
            appointments,
            effectiveRiskScore);
    }
}
