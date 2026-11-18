using PsyLink.Domain.Common;

namespace PsyLink.Domain.Clinical;

public class Appointment : ITenantScoped
{
    /// <summary>Un paciente solo puede reprogramar su cita una vez (HU-09 y README).</summary>
    public const int MaxPatientReschedules = 1;

    public Guid AppointmentId { get; set; }
    public Guid PsychologistId { get; set; }
    public Guid PatientId { get; set; }
    public Guid TenantId { get; set; }
    public DateOnly DateAt { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }

    /// <summary>Columna nueva (no está en el ER original).</summary>
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    /// <summary>Columna nueva: cantidad de veces que se reprogramó la cita.</summary>
    public int RescheduleCount { get; set; }

    public Psychologist Psychologist { get; set; } = null!;
    public Patient Patient { get; set; } = null!;

    public bool CanBeRescheduledByPatient => RescheduleCount < MaxPatientReschedules;

    /// <summary>Indica si esta cita se traslapa con otra del mismo psicólogo.</summary>
    public bool OverlapsWith(Appointment other) =>
        AppointmentId != other.AppointmentId
        && PsychologistId == other.PsychologistId
        && DateAt == other.DateAt
        && StartsAt < other.EndsAt
        && other.StartsAt < EndsAt;

    public void Reschedule(DateOnly dateAt, TimeOnly startsAt, TimeOnly endsAt)
    {
        if (Status is AppointmentStatus.Cancelled or AppointmentStatus.Completed)
            throw new DomainException("No se puede reprogramar una cita cancelada o completada.");

        if (endsAt <= startsAt)
            throw new DomainException("La hora final debe ser posterior a la hora inicial.");

        DateAt = dateAt;
        StartsAt = startsAt;
        EndsAt = endsAt;
        RescheduleCount++;
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            throw new DomainException("Solo se puede confirmar una cita pendiente.");
        Status = AppointmentStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == AppointmentStatus.Completed)
            throw new DomainException("No se puede cancelar una cita completada.");
        Status = AppointmentStatus.Cancelled;
    }

    public void Complete()
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new DomainException("No se puede completar una cita cancelada.");
        Status = AppointmentStatus.Completed;
    }
}
