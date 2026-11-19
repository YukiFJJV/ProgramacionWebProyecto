namespace PsyLink.Application.Features.Users.DTOs;

public sealed record UserCreateRequestDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public IReadOnlyList<string>? PhoneNumbers { get; init; }
    public Guid? RoleId { get; init; }
}
