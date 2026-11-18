using MediatR;
using PsyLink.Application.Features.PatientRecords.DTOs;
using PsyLink.Application.Features.PatientRecords.Services.Commands;
using PsyLink.Application.Features.PatientRecords.Services;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Features.PatientRecords.Services.Handlers.CreatePatientRecord;

public sealed class CreatePatientRecordHandler(
    IPatientRecordRepository recordRepository,
    IPatientRepository patientRepository,
    IAppointmentRepository appointmentRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<CreatePatientRecordCommand, RecordDetailResponseDto>
{
    public async Task<RecordDetailResponseDto> Handle(
        CreatePatientRecordCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = PatientRecordAccess.RequirePsychologistTenant(currentUser);
        var input = request.Record;
        var patient = await patientRepository.GetByIdForTenantAsync(
                input.PatientId,
                tenantId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el paciente {input.PatientId}.");

        if (!await appointmentRepository.HasActiveCareRelationshipAsync(
                patient.PatientId,
                currentUser.UserId,
                tenantId,
                cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "El psicólogo no mantiene una relación asistencial activa con este paciente.");
        }

        if (await recordRepository.ExistsByPatientIdAsync(patient.PatientId, tenantId, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.ConflictException("El paciente ya tiene un expediente clínico.");
        }

        Appointment? appointment = null;
        if (input.AppointmentId is Guid appointmentId)
        {
            appointment = await appointmentRepository.GetByIdForTenantAsync(
                    appointmentId,
                    tenantId,
                    cancellationToken)
                ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró la cita {appointmentId}.");

            if (appointment.PatientId != patient.PatientId ||
                appointment.Psychologist.UserId != currentUser.UserId)
            {
                throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                    "El expediente debe originarse en una cita del paciente atendida por el psicólogo autenticado.");
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                throw new FluentValidation.ValidationException(
                    "No se puede crear un expediente a partir de una cita cancelada.");
            }
        }

        var record = new PatientRecord
        {
            RecordId = Guid.NewGuid(),
            PatientId = patient.PatientId,
            TenantId = tenantId,
            AppointmentId = appointment?.AppointmentId,
            Patient = patient,
            Appointment = appointment
        };
        record.UpdateRiskScore(input.RiskScore);
        if (!await recordRepository.AddIfPatientHasNoRecordAsync(record, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.ConflictException("El paciente ya tiene un expediente clínico.");
        }

        var appointments = appointment is null
            ? Array.Empty<Appointment>()
            : new[] { appointment };
        return PatientRecordDtoMapper.ToDetail(record, appointments);
    }
}
