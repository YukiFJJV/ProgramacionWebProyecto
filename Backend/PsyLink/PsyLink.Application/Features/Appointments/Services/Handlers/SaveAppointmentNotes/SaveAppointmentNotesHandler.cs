using MediatR;
using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Appointments.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Appointments.Services.Handlers.SaveAppointmentNotes;

public sealed class SaveAppointmentNotesHandler(
    IAppointmentRepository appointmentRepository,
    IAppointmentNotesRepository appointmentNotesRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<SaveAppointmentNotesCommand, AppointmentSessionNotesResponseDto>
{
    public async Task<AppointmentSessionNotesResponseDto> Handle(
        SaveAppointmentNotesCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.TenantId is not Guid tenantId ||
            currentUser.RoleName != RoleNames.Psychologist)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("Solo un psicólogo autenticado puede registrar notas de sesión.");
        }

        var appointment = await appointmentRepository.GetByIdForTenantAsync(
                request.AppointmentId,
                tenantId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró la cita {request.AppointmentId}.");

        if (appointment.Psychologist.UserId != currentUser.UserId)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("El psicólogo solo puede editar las notas de sus citas.");
        }

        appointment.Complete();

        var updatedAt = await appointmentNotesRepository.SaveAsync(
            appointment,
            tenantId,
            request.Notes.Notes.Trim(),
            cancellationToken);

        return new AppointmentSessionNotesResponseDto
        {
            AppointmentId = appointment.AppointmentId,
            Notes = request.Notes.Notes.Trim(),
            UpdatedAt = updatedAt
        };
    }
}
