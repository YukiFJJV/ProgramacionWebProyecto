namespace PsyLink.API.DTOs;

/// <summary>Payload for creating or updating a diagnosis in the global catalog.</summary>
public sealed record DiagnosisRequestDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
