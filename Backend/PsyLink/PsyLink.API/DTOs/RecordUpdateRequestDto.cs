namespace PsyLink.API.DTOs;

/// <summary>Fields that may be updated on a clinical record.</summary>
public sealed record RecordUpdateRequestDto
{
    public int? RiskScore { get; init; }
}
