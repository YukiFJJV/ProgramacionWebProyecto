using MediatR;
using PsyLink.Application.Features.PatientRecords.DTOs;
using PsyLink.Application.Features.PatientRecords.Services;
using PsyLink.Application.Features.PatientRecords.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Features.PatientRecords.Services.Handlers.UpdatePatientRecord;

public sealed class UpdatePatientRecordHandler(
    IPatientRecordRepository recordRepository,
    IAppointmentRepository appointmentRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdatePatientRecordCommand, RecordDetailResponseDto>
{
    public async Task<RecordDetailResponseDto> Handle(
        UpdatePatientRecordCommand request,
        CancellationToken cancellationToken)
    {
        var (record, tenantId) = await PatientRecordAccess.GetAuthorizedRecordAsync(
            request.RecordId,
            recordRepository,
            appointmentRepository,
            currentUser,
            cancellationToken);

        if (request.Update.RiskScore is int riskScore)
        {
            record.UpdateRiskScore(riskScore);
            await recordRepository.UpdateAsync(record, cancellationToken);
        }

        var detailedRecord = await recordRepository.GetByPatientIdWithDetailsAsync(
                record.PatientId,
                tenantId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró expediente para el paciente {record.PatientId}.");
        var appointments = await appointmentRepository.GetByPatientAndTenantAsync(
            record.PatientId,
            tenantId,
            cancellationToken);
        return PatientRecordDtoMapper.ToDetail(detailedRecord, appointments);
    }
}
