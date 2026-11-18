namespace PsyLink.API.DTOs;

/// <summary>Access token and session identity returned after successful login.</summary>
public sealed record LoginResponseDto(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    UserSessionDto User);
