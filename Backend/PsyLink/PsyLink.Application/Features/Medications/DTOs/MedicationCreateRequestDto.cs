namespace PsyLink.Application.Features.Medications.DTOs;

public class MedicationCreateRequestDto
{
    public Guid? LaboratoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Content { get; set; }
    public string Presentation { get; set; } = string.Empty;
    public List<Guid> IngredientIds { get; set; } = new();
}
