namespace PsyLink.API.DTOs;

/// <summary>Tenant user details; password credentials are never included.</summary>
public sealed record UserResponseDto(
    Guid UserId,
    Guid TenantId,
    string Email,
    string FirstName,
    string LastName,
    PhoneNumberResponseDto? Phone,
    RoleResponseDto Role);
