namespace PsyLink.Application.Features.Authentication.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
}
