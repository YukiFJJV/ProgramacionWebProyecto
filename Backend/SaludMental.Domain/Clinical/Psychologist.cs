using SaludMental.Domain.Identity;

namespace SaludMental.Domain.Clinical;

public class Psychologist
{
    public Guid PsychologistId { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public Guid SpecialtyId { get; set; }

    /// <summary>Usuario administrador responsable (FK opcional a user).</summary>
    public Guid? AdminId { get; set; }

    /// <summary>Número de colegiación/licencia (CHAR(6)).</summary>
    public string LicenseNumber { get; set; } = string.Empty;

    /// <summary>Columna nueva: HU-15 muestra la biografía del psicólogo al paciente.</summary>
    public string? Biography { get; set; }

    public User User { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
    public Specialty Specialty { get; set; } = null!;
    public User? Admin { get; set; }

    public ICollection<WorkSchedule> Schedules { get; set; } = new List<WorkSchedule>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
