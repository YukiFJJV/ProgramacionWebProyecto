namespace PsyLink.Domain.Clinical;

/// <summary>
/// Estado de la cita. No está en el ER original; se agrega porque la regla de los
/// 10 meses y el cambio de psicólogo necesitan saber qué citas fueron realmente atendidas.
/// </summary>
public enum AppointmentStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Completed = 3
}
