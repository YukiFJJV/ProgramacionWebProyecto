using PsyLink.Domain.Common;

namespace PsyLink.Domain.Clinical;

public class Appointment : ITenantScoped
{
    /// <summary>Un paciente solo puede reprogramar su cita una vez (HU-09 y README).</summary>
    public const int MaxPatientReschedules = 1;

    private Appointment() { }

    private Appointment(
        Guid appointmentId,
        Guid psychologistId,
        Guid patientId,
        Guid tenantId,
        DateOnly dateAt,
        TimeOnly startsAt,
        TimeOnly endsAt)
    {
        AppointmentId = appointmentId;
        PsychologistId = psychologistId;
        PatientId = patientId;
        TenantId = tenantId;

        Reschedule(dateAt, startsAt, endsAt);
        Status = AppointmentStatus.Pending;
        RescheduleCount = 0;
    }

    public static Appointment Schedule(
        Guid psychologistId,
        Guid patientId,
        Guid tenantId,
        DateOnly dateAt,
        TimeOnly startsAt,
        TimeOnly endsAt)
    {
        if (endsAt <= startsAt)
            throw new DomainException("La hora final debe ser posterior a la hora inicial.");

        return new Appointment(
            Guid.NewGuid(),
            psychologistId,
            patientId,
            tenantId,
            dateAt,
            startsAt,
            endsAt);
    }

    public Guid AppointmentId { get; private set; }
    public Guid PsychologistId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid TenantId { get; set; }
    public DateOnly DateAt { get; private set; }
    public TimeOnly StartsAt { get; private set; }
    public TimeOnly EndsAt { get; private set; }

    /// <summary>Columna nueva (no está en el ER original).</summary>
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Pending;

    /// <summary>Columna nueva: cantidad de veces que se reprogramó la cita.</summary>
    public int RescheduleCount { get; private set; }

    public void AssignTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            throw new DomainException("El tenant es obligatorio.");
        if (TenantId != Guid.Empty)
            throw new DomainException("El tenant no puede cambiar una vez asignado.");
        TenantId = tenantId;
    }

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
