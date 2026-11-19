using PsyLink.Application.Common.Exceptions;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Users.Shared;

public static class UserDtoMapper
{
    public static UserResponseDto ToResponse(User user)
    {
        if (user.TenantId is null)
        {
            throw new ConflictException("La cuenta registrada no tiene una empresa asignada.");
        }

        return new UserResponseDto
        {
            UserId = user.UserId,
            TenantId = user.TenantId.Value,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone is null
                ? null
                : new PhoneNumberResponseDto
                {
                    PhoneId = user.Phone.PhoneId,
                    PhoneNumber = user.Phone.Number
                },
            Phones = (user.Phones.Count > 0 ? user.Phones : user.Phone is null
                    ? []
                    : [user.Phone])
                .Select(phone => new PhoneNumberResponseDto
                {
                    PhoneId = phone.PhoneId,
                    PhoneNumber = phone.Number
                })
                .ToList(),
            Role = user.Role is null
                ? new RoleResponseDto()
                : new RoleResponseDto
                {
                    RoleId = user.Role.RoleId,
                    Name = user.Role.Name
                }
        };
    }
}
