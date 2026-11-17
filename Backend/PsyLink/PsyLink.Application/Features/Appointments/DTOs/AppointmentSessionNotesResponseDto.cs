namespace PsyLink.Application.Features.Appointments.DTOs;

public class AppointmentSessionNotesResponseDto
{
    public Guid AppointmentId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}
