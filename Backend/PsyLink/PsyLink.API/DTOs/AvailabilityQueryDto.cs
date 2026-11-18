namespace PsyLink.API.DTOs;

/// <summary>Requested date range and slot settings for psychologist availability.</summary>
public sealed record AvailabilityQueryDto
{
    public DateOnly DateFrom { get; init; }
    public DateOnly DateTo { get; init; }
    public int? DurationMinutes { get; init; } = 60;
    public string Timezone { get; init; } = "America/Tegucigalpa";
}
