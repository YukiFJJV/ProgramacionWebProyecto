using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Patients.DTOs;

public sealed record PatientCreateRequestDto
{
    public UserCreateRequestDto User { get; init; } = new();
    public DateOnly BirthDate { get; init; }
    public string? Description { get; init; }
}
