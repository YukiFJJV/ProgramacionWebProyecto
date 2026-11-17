using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Patients.DTOs;

public class PatientResponseDto
{
    public Guid PatientId { get; set; }
    public UserResponseDto User { get; set; } = new();
    public DateOnly BirthDate { get; set; }
    public string? Description { get; set; }
    public int Age { get; set; }
}
