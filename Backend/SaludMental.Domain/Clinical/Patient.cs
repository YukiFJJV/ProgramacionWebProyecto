using SaludMental.Domain.Identity;

namespace SaludMental.Domain.Clinical;

public class Patient
{
    public Guid PatientId { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? Description { get; set; }

    public User User { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<PatientDiagnosis> Diagnoses { get; set; } = new List<PatientDiagnosis>();

    /// <summary>Edad calculada; no se persiste.</summary>
    public int AgeOn(DateOnly today)
    {
        var age = today.Year - BirthDate.Year;
        if (BirthDate > today.AddYears(-age)) age--;
        return age;
    }
}
