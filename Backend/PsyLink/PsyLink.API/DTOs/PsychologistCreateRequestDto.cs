namespace PsyLink.API.DTOs;

/// <summary>Account and professional data for registering a tenant psychologist.</summary>
public sealed record PsychologistCreateRequestDto
{
    public required UserCreateRequestDto User { get; init; }
    public required Guid SpecialtyId { get; init; }
    public required string LicenseNumber { get; init; }
    public Guid? AdminId { get; init; }
}
