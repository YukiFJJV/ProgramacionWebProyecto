using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Psychologists.DTOs;

public class PsychologistCreateRequestDto
{
    public UserCreateRequestDto User { get; set; } = new();
    public Guid SpecialtyId { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public Guid? AdminId { get; set; }
}
