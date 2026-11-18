namespace PsyLink.API.DTOs;

/// <summary>Phone number associated with a tenant.</summary>
public sealed record PhoneNumberResponseDto(Guid PhoneId, string PhoneNumber);
