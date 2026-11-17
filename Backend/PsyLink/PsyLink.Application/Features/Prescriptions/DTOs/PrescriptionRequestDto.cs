namespace PsyLink.Application.Features.Prescriptions.DTOs;

public class PrescriptionRequestDto
{
    public Guid MedicationId { get; set; }
    public string Dosage { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
