using System.ComponentModel.DataAnnotations;

namespace PsyLink.API.DTOs;

/// <summary>Account data and assignable role for creating a tenant user.</summary>
public sealed record UserCreateRequestDto
{
    [Required, EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }

    [Required]
    public required string FirstName { get; init; }

    [Required]
    public required string LastName { get; init; }

    public string? PhoneNumber { get; init; }

    [Required]
    public required Guid RoleId { get; init; }
}
