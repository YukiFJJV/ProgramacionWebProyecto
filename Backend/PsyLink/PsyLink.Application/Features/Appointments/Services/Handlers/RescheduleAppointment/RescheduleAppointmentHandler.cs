using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Appointments.Services.Commands;
using PsyLink.Application.Features.Appointments.Services.Handlers.CreateAppointment;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Appointments.Services.Handlers.RescheduleAppointment;

public sealed class RescheduleAppointmentHandler(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IPsychologistRepository psychologistRepository,
    IWorkScheduleRepository workScheduleRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<RescheduleAppointmentCommand, AppointmentResponseDto>
{
    public async Task<AppointmentResponseDto> Handle(
        RescheduleAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = currentUser.TenantId;
        if (tenantId is null ||
            (currentUser.RoleName != RoleNames.Admin && currentUser.RoleName != RoleNames.Patient))
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere un administrador o paciente autenticado.");
        }

        var appointment = await appointmentRepository.GetByIdForTenantAsync(
                request.AppointmentId,
                tenantId.Value,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró la cita {request.AppointmentId}.");

        var isPatient = currentUser.RoleName == RoleNames.Patient;
        if (isPatient)
        {
            var patient = await patientRepository.GetByUserIdAsync(
                    currentUser.UserId,
                    tenantId.Value,
                    cancellationToken)
                ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException("No se encontró el perfil de paciente autenticado.");
            if (appointment.PatientId != patient.PatientId)
            {
                throw new PsyLink.Application.Common.Exceptions.ForbiddenException("Un paciente solo puede reprogramar sus propias citas.");
            }

            if (!appointment.CanBeRescheduledByPatient)
            {
                throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El paciente solo puede reprogramar cada cita una vez.");
            }
        }

        var targetPsychologistId = request.Update.PsychologistId ?? appointment.PsychologistId;
        var psychologist = await psychologistRepository.GetByIdForTenantAsync(
                targetPsychologistId,
                tenantId.Value,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el psicólogo {targetPsychologistId}.");
        var dateAt = request.Update.DateAt ?? appointment.DateAt;
        var startsAt = request.Update.StartsAt ?? appointment.StartsAt;
        var endsAt = request.Update.EndsAt ?? appointment.EndsAt;

        if (endsAt <= startsAt)
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("La hora final debe ser posterior a la hora inicial.");
        }

        var schedules = await workScheduleRepository.GetByPsychologistAsync(
            targetPsychologistId,
            tenantId.Value,
            cancellationToken);
        if (!schedules.Any(schedule => schedule.Covers(dateAt.ToWeekDay(), startsAt, endsAt)))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("La cita debe estar dentro del horario laboral del psicólogo.");
        }

        if (await appointmentRepository.HasOverlapAsync(
                targetPsychologistId,
                dateAt,
                startsAt,
                endsAt,
                appointment.AppointmentId,
                cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El horario seleccionado se traslapa con otra cita.");
        }

        appointment.Reschedule(dateAt, startsAt, endsAt);
        appointment.PsychologistId = targetPsychologistId;
        appointment.Psychologist = psychologist;

        if (!await appointmentRepository.UpdateIfNoOverlapAsync(appointment, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("El horario seleccionado se traslapa con otra cita.");
        }

        return CreateAppointmentHandler.ToResponse(appointment);
    }
}
