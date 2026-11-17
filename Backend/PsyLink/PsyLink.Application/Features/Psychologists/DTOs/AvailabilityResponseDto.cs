namespace PsyLink.Application.Features.Psychologists.DTOs;

public class AvailabilityResponseDto
{
    public Guid PsychologistId { get; set; }
    public DateOnly Date { get; set; }
    public List<AvailabilitySlotDto> Slots { get; set; } = new();
}
