namespace PsyLink.API.DTOs;

/// <summary>Specialty assigned to a psychologist.</summary>
public sealed record SpecialtyResponseDto(Guid SpecialtyId, string SpecialtyName);
