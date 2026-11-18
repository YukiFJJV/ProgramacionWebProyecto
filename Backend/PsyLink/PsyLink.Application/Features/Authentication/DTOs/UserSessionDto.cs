using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Authentication.DTOs;

public class UserSessionDto
{
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public RoleResponseDto Role { get; set; } = new();
}
