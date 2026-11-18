namespace PsyLink.API.DTOs;

/// <summary>Complete diagnosis catalog entry.</summary>
public sealed record DiagnosisDetailResponseDto(
    Guid DiagnosisId,
    string Name,
    string? Description);
