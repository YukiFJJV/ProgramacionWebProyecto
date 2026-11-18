namespace PsyLink.Application.Features.Users.DTOs;

public sealed record UserUpdateRequestDto(
    string? Email,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    Guid? RoleId,
    bool? Active);
