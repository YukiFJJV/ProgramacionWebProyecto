namespace PsyLink.API.DTOs;

/// <summary>Account and clinical profile data for registering a tenant patient.</summary>
public sealed record PatientCreateRequestDto
{
    public required UserCreateRequestDto User { get; init; }
    public DateOnly BirthDate { get; init; }
    public string? Description { get; init; }
}
