namespace PsyLink.API.DTOs;

/// <summary>Laboratory included in a medication response.</summary>
public sealed record LaboratoryResponseDto(
    Guid LaboratoryId,
    string Name,
    PhoneNumberResponseDto? Phone);
