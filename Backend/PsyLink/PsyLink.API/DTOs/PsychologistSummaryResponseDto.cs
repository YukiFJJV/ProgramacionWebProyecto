namespace PsyLink.API.DTOs;

/// <summary>Summary information for a tenant psychologist.</summary>
public sealed record PsychologistSummaryResponseDto(
    Guid PsychologistId,
    string FirstName,
    string LastName,
    SpecialtyResponseDto Specialty,
    string LicenseNumber);
