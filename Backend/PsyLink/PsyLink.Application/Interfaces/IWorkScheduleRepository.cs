using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Interfaces;

public interface IWorkScheduleRepository
{
    Task<WorkSchedule?> GetByIdAsync(Guid scheduleId, CancellationToken cancellationToken = default);

    Task<WorkSchedule?> GetByIdForTenantAsync(
        Guid scheduleId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkSchedule>> GetByPsychologistAsync(
        Guid psychologistId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkSchedule>> GetOverlappingAsync(
        Guid psychologistId,
        Guid tenantId,
        WeekDay weekDay,
        TimeOnly startsAt,
        TimeOnly endsAt,
        Guid? excludingScheduleId = null,
        CancellationToken cancellationToken = default);

    /// <summary>Persists only if no overlapping block exists, checking atomically.</summary>
    Task<bool> AddIfNoOverlapAsync(
        WorkSchedule schedule,
        CancellationToken cancellationToken = default);

    /// <summary>Updates only if no overlapping block exists, checking atomically.</summary>
    Task<bool> UpdateIfNoOverlapAsync(
        WorkSchedule schedule,
        CancellationToken cancellationToken = default);
}
