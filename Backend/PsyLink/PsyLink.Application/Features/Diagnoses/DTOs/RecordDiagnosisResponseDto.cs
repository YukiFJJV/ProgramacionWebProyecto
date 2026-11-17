namespace PsyLink.Application.Features.Diagnoses.DTOs;

public class RecordDiagnosisResponseDto
{
    public Guid RecordId { get; set; }
    public DiagnosisDetailResponseDto Diagnosis { get; set; } = new();
    public string? Notes { get; set; }
}
