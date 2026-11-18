namespace PsyLink.API.DTOs;

/// <summary>Credentials submitted to authenticate a registered user.</summary>
public sealed record LoginRequestDto(string Email, string Password);
