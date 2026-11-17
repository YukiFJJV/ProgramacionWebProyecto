namespace PsyLink.Application.Features.Prescriptions.DTOs;

public class PrescriptionUpdateRequestDto
{
    public Guid? MedicationId { get; set; }
    public string? Dosage { get; set; }
    public int? Quantity { get; set; }
}
