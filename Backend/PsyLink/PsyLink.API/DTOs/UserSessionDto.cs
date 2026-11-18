namespace PsyLink.API.DTOs;

/// <summary>Non-sensitive user identity and role returned after login.</summary>
public sealed record UserSessionDto(
    Guid UserId,
    Guid? TenantId,
    string Email,
    string FirstName,
    string LastName,
    RoleResponseDto Role);
