using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Appointments.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Appointments.Services.Handlers.CreateAppointment;

public sealed class CreateAppointmentHandler(
    IPsychologistRepository psychologistRepository,
    IPatientRepository patientRepository,
    IPatientRecordRepository patientRecordRepository,
    IWorkScheduleRepository workScheduleRepository,
    IAppointmentRepository appointmentRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateAppointmentCommand, AppointmentResponseDto>
{
    public async Task<AppointmentResponseDto> Handle(
        CreateAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = currentUser.TenantId;
        if (tenantId is null ||
            (currentUser.RoleName != RoleNames.Patient &&
             currentUser.RoleName != RoleNames.Psychologist))
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La programación requiere un paciente o psicólogo autenticado.");
        }

        var input = request.Appointment;
        if (input.EndsAt <= input.StartsAt)
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("La hora final debe ser posterior a la hora inicial.");
        }

        var psychologist = await psychologistRepository.GetByIdForTenantAsync(
                input.PsychologistId,
                tenantId.Value,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el psicólogo {input.PsychologistId}.");
        if (!psychologist.User.Active)
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("No se pueden programar citas con un psicólogo inactivo.");
        }

        Patient patient;
        if (currentUser.RoleName == RoleNames.Patient)
        {
            if (input.PatientId.HasValue)
            {
                throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El paciente de la cita se obtiene de la sesión autenticada.");
            }

            patient = await patientRepository.GetByUserIdAsync(currentUser.UserId, tenantId.Value, cancellationToken)
                ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException("No se encontró el perfil de paciente autenticado.");
        }
        else
        {
            if (input.PatientId is null)
            {
                throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El psicólogo debe especificar el paciente para la cita.");
            }

            if (psychologist.UserId != currentUser.UserId)
            {
                throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                    "Un psicólogo solo puede programar citas en su propia agenda.");
            }

            patient = await patientRepository.GetByIdForTenantAsync(
                    input.PatientId.Value,
                    tenantId.Value,
                    cancellationToken)
                ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el paciente {input.PatientId}.");

            var record = await patientRecordRepository.GetByPatientIdAsync(
                patient.PatientId,
                tenantId.Value,
                cancellationToken);
            if (record?.RiskScore != RiskLevel.Urgent)
            {
                throw new PsyLink.Application.Common.Exceptions.BusinessRuleException(
                    "El psicólogo solo puede programar directamente para pacientes con nivel de riesgo 3.");
            }
        }

        if (!patient.User.Active)
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("No se puede programar una cita para un paciente inactivo.");
        }

        var schedules = await workScheduleRepository.GetByPsychologistAsync(
            psychologist.PsychologistId,
            tenantId.Value,
            cancellationToken);
        if (!schedules.Any(schedule =>
                schedule.Covers(input.DateAt.ToWeekDay(), input.StartsAt, input.EndsAt)))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("La cita debe estar dentro del horario laboral del psicólogo.");
        }

        if (await appointmentRepository.HasOverlapAsync(
                psychologist.PsychologistId,
                input.DateAt,
                input.StartsAt,
                input.EndsAt,
                cancellationToken: cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El horario seleccionado se traslapa con otra cita.");
        }

        var appointment = Appointment.Schedule(
            psychologist.PsychologistId,
            patient.PatientId,
            tenantId.Value,
            input.DateAt,
            input.StartsAt,
            input.EndsAt);
        if (!await appointmentRepository.AddIfNoOverlapAsync(appointment, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El horario seleccionado se traslapa con otra cita.");
        }

        return ToResponse(appointment);
    }

    internal static AppointmentResponseDto ToResponse(Appointment appointment) => new()
    {
        AppointmentId = appointment.AppointmentId,
        PatientId = appointment.PatientId,
        PatientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}",
        PsychologistId = appointment.PsychologistId,
        PsychologistName = $"{appointment.Psychologist.User.FirstName} {appointment.Psychologist.User.LastName}",
        DateAt = appointment.DateAt,
        StartsAt = appointment.StartsAt,
        EndsAt = appointment.EndsAt
    };
}
