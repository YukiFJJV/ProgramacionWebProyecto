using MediatR;
using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Appointments.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Appointments.Services.Handlers.GetAppointmentDetail;

public sealed class GetAppointmentDetailHandler(
    IAppointmentRepository appointmentRepository,
    IAppointmentNotesRepository appointmentNotesRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetAppointmentDetailQuery, AppointmentDetailResponseDto>
{
    public async Task<AppointmentDetailResponseDto> Handle(
        GetAppointmentDetailQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.TenantId is not Guid tenantId ||
            currentUser.RoleName != RoleNames.Psychologist)
        {
            throw new UnauthorizedAccessException("Solo un psicólogo autenticado puede consultar estas citas.");
        }

        var appointment = await appointmentRepository.GetByIdForTenantAsync(
                request.AppointmentId,
                tenantId,
                cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró la cita {request.AppointmentId}.");

        if (appointment.Psychologist.UserId != currentUser.UserId)
        {
            throw new UnauthorizedAccessException("El psicólogo solo puede consultar sus propias citas.");
        }

        var response = AppointmentDtoMapper.ToDetail(appointment);
        var notes = await appointmentNotesRepository.GetByAppointmentIdAsync(
            appointment.AppointmentId,
            tenantId,
            cancellationToken);
        response.SessionNotes = notes?.Notes;
        response.NotesUpdatedAt = notes?.UpdatedAt;
        return response;
    }
}
