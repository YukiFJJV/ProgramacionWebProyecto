namespace PsyLink.API.DTOs;

/// <summary>Patient profile and its associated account details.</summary>
public sealed record PatientResponseDto(
    Guid PatientId,
    UserResponseDto User,
    DateOnly BirthDate,
    string? Description,
    int Age);
