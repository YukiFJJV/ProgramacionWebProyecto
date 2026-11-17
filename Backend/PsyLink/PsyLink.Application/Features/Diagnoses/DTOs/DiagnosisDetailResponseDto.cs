namespace PsyLink.Application.Features.Diagnoses.DTOs;

public class DiagnosisDetailResponseDto
{
    public Guid DiagnosisId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
