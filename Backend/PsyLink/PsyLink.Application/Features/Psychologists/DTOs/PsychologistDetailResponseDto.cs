using PsyLink.Application.Features.Specialties.DTOs;
using PsyLink.Application.Features.Schedules.DTOs;
using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Psychologists.DTOs;

public class PsychologistDetailResponseDto
{
    public Guid PsychologistId { get; set; }
    public UserResponseDto User { get; set; } = new();
    public SpecialtyResponseDto Specialty { get; set; } = new();
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public List<ScheduleResponseDto> Schedules { get; set; } = new();
}
