namespace PsyLink.API.DTOs;

/// <summary>Start and end instants for an available appointment slot.</summary>
public sealed record AvailabilitySlotDto(DateTimeOffset StartsAt, DateTimeOffset EndsAt);
