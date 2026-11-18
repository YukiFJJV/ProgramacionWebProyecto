namespace PsyLink.Application.Features.Users.DTOs;

public sealed record PhoneNumberResponseDto
{
    public Guid PhoneId { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
}
