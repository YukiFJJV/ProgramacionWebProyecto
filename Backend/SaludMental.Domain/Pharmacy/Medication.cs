namespace SaludMental.Domain.Pharmacy;

public class Medication
{
    public Guid MedicationId { get; set; }
    public Guid? LaboratoryId { get; set; }

    /// <summary>Nombre comercial o genérico.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Contenido o cantidad por presentación (mayor que 0).</summary>
    public int Content { get; set; }

    /// <summary>Por ejemplo: tabletas, solución.</summary>
    public string Presentation { get; set; } = string.Empty;

    public Laboratory? Laboratory { get; set; }
    public ICollection<MedicationIngredient> Ingredients { get; set; } = new List<MedicationIngredient>();
}
