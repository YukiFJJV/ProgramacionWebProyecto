namespace PsyLink.API.DTOs;

/// <summary>Tenant data returned by registration and administration queries.</summary>
public sealed record TenantResponseDto(
    Guid TenantId,
    string Name,
    string? Email,
    string? Rtn,
    PhoneNumberResponseDto? Phone,
    bool Active);
