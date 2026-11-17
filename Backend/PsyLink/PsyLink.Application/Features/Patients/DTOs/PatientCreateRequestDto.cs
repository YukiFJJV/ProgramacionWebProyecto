namespace PsyLink.Application.Features.Patients.DTOs;

public class PatientCreateRequestDto
{
    public Guid UserId { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? Description { get; set; }
}
