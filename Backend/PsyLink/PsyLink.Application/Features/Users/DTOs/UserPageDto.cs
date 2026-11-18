namespace PsyLink.Application.Features.Users.DTOs;

public class UserPageDto
{
    public List<UserResponseDto> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
