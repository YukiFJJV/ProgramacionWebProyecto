namespace PsyLink.Application.Features.Users.DTOs;

public class UserResponseDto
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public PhoneNumberResponseDto? Phone { get; set; }
    public RoleResponseDto Role { get; set; } = new();
}
