using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Prescriptions.DTOs;

public class LaboratoryResponseDto
{
    public Guid LaboratoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public PhoneNumberResponseDto? Phone { get; set; }
}
