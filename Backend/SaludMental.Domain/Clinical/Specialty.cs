namespace SaludMental.Domain.Clinical;

public class Specialty
{
    public Guid SpecialtyId { get; set; }

    /// <summary>Nombre único.</summary>
    public string SpecialtyName { get; set; } = string.Empty;
}
