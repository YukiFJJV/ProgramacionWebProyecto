using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Tenants.DTOs;

public sealed record TenantResponseDto(
    Guid TenantId,
    string Name,
    string? Email,
    string? Rtn,
    PhoneNumberResponseDto? Phone,
    bool Active);
