namespace PsyLink.Application.Features.Psychologists.DTOs;

public class AvailabilityQueryDto
{
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public int? DurationMinutes { get; set; }
    public string Timezone { get; set; } = "America/Tegucigalpa";
}
