namespace PsyLink.Application.Features.Users.DTOs;

public class PhoneNumberResponseDto
{
    public Guid PhoneId { get; set; }
    public string Number { get; set; } = string.Empty;
}
