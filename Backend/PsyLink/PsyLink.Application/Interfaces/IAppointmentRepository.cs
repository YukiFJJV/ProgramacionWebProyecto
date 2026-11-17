using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Interfaces;

public interface IAppointmentRepository
{
    /// <summary>
    /// Returns the appointment with patient user/role/phone and psychologist
    /// user/specialty details loaded.
    /// </summary>
    Task<Appointment?> GetByIdForTenantAsync(
        Guid appointmentId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>Checks conflicts with non-cancelled appointments for the psychologist.</summary>
    Task<bool> HasOverlapAsync(
        Guid psychologistId,
        DateOnly dateAt,
        TimeOnly startsAt,
        TimeOnly endsAt,
        Guid? excludingAppointmentId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Appointment>> GetByPsychologistAndDateRangeAsync(
        Guid psychologistId,
        DateOnly dateFrom,
        DateOnly dateTo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns non-cancelled appointments for the psychologist, with patient and
    /// user details loaded, filtered and paginated by the adapter.
    /// </summary>
    Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetByPsychologistWithPatientSearchAsync(
        Guid psychologistId,
        Guid tenantId,
        string? patientSearch,
        Guid? patientId,
        DateOnly? dateFrom,
        DateOnly? dateTo,
        int page,
        int pageSize,
        bool descending,
        CancellationToken cancellationToken = default);

    /// <summary>Loads all patient appointments in the tenant and their display relationships.</summary>
    Task<IReadOnlyList<Appointment>> GetByPatientAndTenantAsync(
        Guid patientId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks that the psychologist user owns the patient's latest non-cancelled
    /// appointment in this tenant.
    /// </summary>
    Task<bool> HasActiveCareRelationshipAsync(
        Guid patientId,
        Guid psychologistUserId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>Persists only if no non-cancelled appointment overlaps, checking atomically.</summary>
    Task<bool> AddIfNoOverlapAsync(
        Appointment appointment,
        CancellationToken cancellationToken = default);

    /// <summary>Updates only if no other non-cancelled appointment overlaps, checking atomically.</summary>
    Task<bool> UpdateIfNoOverlapAsync(
        Appointment appointment,
        CancellationToken cancellationToken = default);
}
