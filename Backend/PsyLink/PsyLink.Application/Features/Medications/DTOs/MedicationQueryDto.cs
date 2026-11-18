namespace PsyLink.Application.Features.Medications.DTOs;

public class MedicationQueryDto
{
    public string? Search { get; set; }
    public Guid? LaboratoryId { get; set; }
    public Guid? IngredientId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
