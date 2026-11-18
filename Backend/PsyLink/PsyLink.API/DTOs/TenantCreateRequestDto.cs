namespace PsyLink.API.DTOs;

/// <summary>Data used to register a tenant.</summary>
public sealed record TenantCreateRequestDto(
    string Name,
    string? Email,
    string? Rtn,
    string? PhoneNumber);
