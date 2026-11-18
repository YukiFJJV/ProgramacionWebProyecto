using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Psychologists.DTOs;

public sealed record PsychologistCreateRequestDto
{
    public UserCreateRequestDto User { get; init; } = new();
    public Guid SpecialtyId { get; init; }
    public string LicenseNumber { get; init; } = string.Empty;
    public Guid? AdminId { get; init; }
}
