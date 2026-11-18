using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Patients.DTOs;

public class PatientCreateRequestDto
{
    public UserCreateRequestDto User { get; set; } = new();
    public DateOnly BirthDate { get; set; }
    public string? Description { get; set; }
}
