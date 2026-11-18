using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Common.Exceptions;
using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;
using PsyLink.Infrastructure.Persistence.Entities.Clinical;

namespace PsyLink.Infrastructure.Repositories.Clinical;

public sealed class AppointmentNotesRepository(
    ApplicationDbContext context) : IAppointmentNotesRepository
{
    public Task<AppointmentSessionNotesResponseDto?> GetByAppointmentIdAsync(
        Guid appointmentId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return context.AppointmentSessionNotes
            .AsNoTracking()
            .Where(notes =>
                notes.AppointmentId == appointmentId &&
                notes.TenantId == tenantId)
            .Select(notes => new AppointmentSessionNotesResponseDto
            {
                AppointmentId = notes.AppointmentId,
                Notes = notes.Notes,
                UpdatedAt = notes.UpdatedAt
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<DateTime> SaveAsync(
        Appointment appointment,
        Guid tenantId,
        string notes,
        CancellationToken cancellationToken = default)
    {
        if (appointment.TenantId != tenantId)
        {
            throw new ForbiddenException("La cita no pertenece al tenant autenticado.");
        }

        if (string.IsNullOrWhiteSpace(notes))
        {
            throw new BusinessRuleException("Las notas de sesión no pueden estar vacías.");
        }

        var existingNotes = await context.AppointmentSessionNotes
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(
                item => item.AppointmentId == appointment.AppointmentId,
                cancellationToken);

        if (existingNotes is not null && existingNotes.TenantId != tenantId)
        {
            throw new ConflictException(
                "Las notas de la cita están asociadas a un tenant distinto.");
        }

        var updatedAt = DateTime.UtcNow;
        if (existingNotes is null)
        {
            context.AppointmentSessionNotes.Add(new AppointmentSessionNotes
            {
                AppointmentId = appointment.AppointmentId,
                TenantId = tenantId,
                Notes = notes,
                UpdatedAt = updatedAt
            });
        }
        else
        {
            existingNotes.Notes = notes;
            existingNotes.UpdatedAt = updatedAt;
        }

        var appointmentEntry = context.Entry(appointment);
        if (appointmentEntry.State == EntityState.Detached)
        {
            context.Attach(appointment);
        }

        context.Entry(appointment)
            .Property(item => item.Status)
            .IsModified = true;

        return updatedAt;
    }
}
