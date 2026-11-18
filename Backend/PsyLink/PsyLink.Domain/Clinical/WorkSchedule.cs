using PsyLink.Domain.Common;

namespace PsyLink.Domain.Clinical;

/// <summary>Bloque de horario laboral semanal de un psicólogo.</summary>
public class WorkSchedule : ITenantScoped
{
    private WorkSchedule() { }

    private WorkSchedule(
        Guid scheduleId,
        Guid psychologistId,
        Guid tenantId,
        WeekDay weekDay,
        TimeOnly startsAt,
        TimeOnly endsAt)
    {
        if (endsAt <= startsAt)
            throw new DomainException("La hora final debe ser posterior a la hora inicial.");

        ScheduleId = scheduleId;
        PsychologistId = psychologistId;
        TenantId = tenantId;
        WeekDay = weekDay;
        StartsAt = startsAt;
        EndsAt = endsAt;
    }

    public static WorkSchedule Create(
        Guid psychologistId,
        Guid tenantId,
        WeekDay weekDay,
        TimeOnly startsAt,
        TimeOnly endsAt) =>
        new(Guid.NewGuid(), psychologistId, tenantId, weekDay, startsAt, endsAt);

    public Guid ScheduleId { get; private set; }
    public Guid PsychologistId { get; private set; }
    public Guid TenantId { get; set; }
    public WeekDay WeekDay { get; private set; }
    public TimeOnly StartsAt { get; private set; }
    public TimeOnly EndsAt { get; private set; }

    public void AssignTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            throw new DomainException("El tenant es obligatorio.");
        if (TenantId != Guid.Empty)
            throw new DomainException("El tenant no puede cambiar una vez asignado.");
        TenantId = tenantId;
    }

    public Psychologist Psychologist { get; set; } = null!;

    /// <summary>Indica si el bloque contiene por completo el rango pedido en ese día.</summary>
    public bool Covers(WeekDay day, TimeOnly startsAt, TimeOnly endsAt) =>
        WeekDay == day && startsAt >= StartsAt && endsAt <= EndsAt;

    /// <summary>Indica si dos bloques del mismo psicólogo y día se traslapan (HU-08).</summary>
    public bool OverlapsWith(WorkSchedule other) =>
        PsychologistId == other.PsychologistId
        && WeekDay == other.WeekDay
        && StartsAt < other.EndsAt
        && other.StartsAt < EndsAt;

    public void Update(WeekDay weekDay, TimeOnly startsAt, TimeOnly endsAt)
    {
        if (endsAt <= startsAt)
            throw new DomainException("La hora final debe ser posterior a la hora inicial.");

        WeekDay = weekDay;
        StartsAt = startsAt;
        EndsAt = endsAt;
    }
}
