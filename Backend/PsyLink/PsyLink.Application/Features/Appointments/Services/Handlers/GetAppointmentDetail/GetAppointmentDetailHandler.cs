using MediatR;
using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Appointments.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Appointments.Services.Handlers.GetAppointmentDetail;

public sealed class GetAppointmentDetailHandler(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IPsychologistRepository psychologistRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetAppointmentDetailQuery, AppointmentDetailResponseDto>
{
    public async Task<AppointmentDetailResponseDto> Handle(
        GetAppointmentDetailQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.TenantId is not Guid tenantId ||
            (currentUser.RoleName != RoleNames.Admin &&
             currentUser.RoleName != RoleNames.Psychologist &&
             currentUser.RoleName != RoleNames.Patient))
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere un administrador, paciente o psicólogo.");
        }

        var appointment = await appointmentRepository.GetByIdForTenantAsync(
                request.AppointmentId,
                tenantId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró la cita {request.AppointmentId}.");

        if (currentUser.RoleName == RoleNames.Psychologist)
        {
            var psychologist = await psychologistRepository.GetByUserIdForTenantAsync(
                    currentUser.UserId,
                    tenantId,
                    cancellationToken)
                ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException("No se encontró el perfil de psicólogo autenticado.");
            if (appointment.PsychologistId != psychologist.PsychologistId)
            {
                throw new PsyLink.Application.Common.Exceptions.ForbiddenException("El psicólogo solo puede consultar sus propias citas.");
            }
        }
        else if (currentUser.RoleName == RoleNames.Patient)
        {
            var patient = await patientRepository.GetByUserIdAsync(
                    currentUser.UserId,
                    tenantId,
                    cancellationToken)
                ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException("No se encontró el perfil de paciente autenticado.");
            if (appointment.PatientId != patient.PatientId)
            {
                throw new PsyLink.Application.Common.Exceptions.ForbiddenException("El paciente solo puede consultar sus propias citas.");
            }
        }

        return AppointmentDtoMapper.ToDetail(appointment);
    }
}
