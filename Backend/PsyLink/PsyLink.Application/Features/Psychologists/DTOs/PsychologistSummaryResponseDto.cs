using PsyLink.Application.Features.Specialties.DTOs;

namespace PsyLink.Application.Features.Psychologists.DTOs;

public class PsychologistSummaryResponseDto
{
    public Guid PsychologistId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public SpecialtyResponseDto Specialty { get; set; } = new();
    public string LicenseNumber { get; set; } = string.Empty;
}
