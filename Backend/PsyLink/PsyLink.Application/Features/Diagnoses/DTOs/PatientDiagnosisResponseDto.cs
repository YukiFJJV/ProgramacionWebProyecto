using PsyLink.Application.Features.Diagnoses.DTOs;

namespace PsyLink.Application.Features.Diagnoses.DTOs;

public class PatientDiagnosisResponseDto
{
    public Guid PatientId { get; set; }
    public DiagnosisDetailResponseDto Diagnosis { get; set; } = new();
    public DiagnosisStatusResponseDto Status { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}
