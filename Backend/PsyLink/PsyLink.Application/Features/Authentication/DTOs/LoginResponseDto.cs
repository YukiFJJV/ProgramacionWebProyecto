namespace PsyLink.Application.Features.Authentication.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public UserSessionDto User { get; set; } = new();
}
