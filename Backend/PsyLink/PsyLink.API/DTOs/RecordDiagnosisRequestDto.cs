namespace PsyLink.API.DTOs;

/// <summary>Notes for a diagnosis associated with a clinical record.</summary>
public sealed record RecordDiagnosisRequestDto
{
    public string? Notes { get; init; }
}
