using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Interfaces;

/// <summary>
/// Persistence contract for session notes. The domain model has no notes entity;
/// an infrastructure adapter must provide the dedicated persistence store.
/// </summary>
public interface IAppointmentNotesRepository
{
    Task<AppointmentSessionNotesResponseDto?> GetByAppointmentIdAsync(
        Guid appointmentId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the session notes and the appointment's completed status atomically.
    /// </summary>
    Task<DateTime> SaveAsync(
        Appointment appointment,
        Guid tenantId,
        string notes,
        CancellationToken cancellationToken = default);
}
