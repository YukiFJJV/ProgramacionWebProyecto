namespace PsyLink.API.DTOs;

/// <summary>Medication and instructions for a new clinical-record prescription.</summary>
public sealed record PrescriptionRequestDto
{
    public Guid MedicationId { get; init; }
    public string Dosage { get; init; } = string.Empty;
    public int Quantity { get; init; }
}
