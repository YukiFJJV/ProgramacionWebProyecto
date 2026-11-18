namespace PsyLink.Application.Features.Patients.DTOs;

public sealed record PatientUpdateRequestDto(
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber,
    DateOnly? BirthDate,
    string? Description);
