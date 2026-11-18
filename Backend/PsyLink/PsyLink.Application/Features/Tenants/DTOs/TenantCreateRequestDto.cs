namespace PsyLink.Application.Features.Tenants.DTOs;

public sealed record TenantCreateRequestDto(
    string Name,
    string? Email,
    string? Rtn,
    string? PhoneNumber);
