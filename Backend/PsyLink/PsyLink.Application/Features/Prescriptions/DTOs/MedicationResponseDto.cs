namespace PsyLink.Application.Features.Prescriptions.DTOs;

public class MedicationResponseDto
{
    public Guid MedicationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Content { get; set; }
    public string Presentation { get; set; } = string.Empty;
    public LaboratoryResponseDto? Laboratory { get; set; }
    public List<IngredientResponseDto> Ingredients { get; set; } = new();
}
