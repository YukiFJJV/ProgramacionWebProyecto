using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Psychologists.DTOs;
using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Features.Appointments.DTOs;

public class AppointmentDetailResponseDto
{
    public Guid AppointmentId { get; set; }
    public PatientResponseDto Patient { get; set; } = new();
    public PsychologistSummaryResponseDto Psychologist { get; set; } = new();
    public DateOnly DateAt { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? SessionNotes { get; set; }
    public DateTime? NotesUpdatedAt { get; set; }
}
