namespace PsyLink.Application.Features.Prescriptions.DTOs;

public class PrescriptionResponseDto
{
    public Guid PrescriptionId { get; set; }
    public Guid RecordId { get; set; }
    public MedicationResponseDto Medication { get; set; } = new();
    public string? Dosage { get; set; }
    public int? Quantity { get; set; }
}
