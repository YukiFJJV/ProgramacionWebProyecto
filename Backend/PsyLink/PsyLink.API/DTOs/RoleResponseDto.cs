namespace PsyLink.API.DTOs;

/// <summary>Role data included in authenticated session responses.</summary>
public sealed record RoleResponseDto(Guid RoleId, string Name);
