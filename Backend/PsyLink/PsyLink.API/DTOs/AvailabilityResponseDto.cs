namespace PsyLink.API.DTOs;

/// <summary>Available appointment slots for a psychologist on a given date.</summary>
public sealed record AvailabilityResponseDto(
    Guid PsychologistId,
    DateOnly Date,
    IReadOnlyList<AvailabilitySlotDto> Slots);
