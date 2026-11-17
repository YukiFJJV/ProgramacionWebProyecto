namespace SaludMental.Domain.Clinical;

/// <summary>Bloque de horario laboral semanal de un psicólogo.</summary>
public class WorkSchedule
{
    public Guid ScheduleId { get; set; }
    public Guid PsychologistId { get; set; }
    public Guid TenantId { get; set; }
    public WeekDay WeekDay { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }

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
}
