using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.Users.Services.Commands;
using PsyLink.Application.Features.Users.Services.Handlers.GetUsers;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Users.Services.Handlers.UpdateUser;

public sealed class UpdateUserHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateUserCommand, UserResponseDto>
{
    public async Task<UserResponseDto> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = GetUsersHandler.EnsureCompanyAdmin(currentUser);
        var user = await userRepository.GetByIdForTenantForUpdateAsync(
            request.UserId,
            tenantId,
            cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el usuario {request.UserId}.");

        var role = user.Role;
        if (request.EmailSpecified)
        {
            user.Email = request.Email!;
        }

        if (request.FirstNameSpecified)
        {
            user.FirstName = request.FirstName!;
        }

        if (request.LastNameSpecified)
        {
            user.LastName = request.LastName!;
        }

        if (request.PhoneNumberSpecified)
        {
            if (request.PhoneNumber is null)
            {
                user.PhoneId = null;
                user.Phone = null;
            }
            else if (user.Phone is null)
            {
                user.Phone = new PhoneNumber
                {
                    PhoneId = Guid.NewGuid(),
                    Number = request.PhoneNumber
                };
                user.PhoneId = user.Phone.PhoneId;
            }
            else
            {
                user.Phone.Number = request.PhoneNumber;
            }
        }

        if (request.RoleIdSpecified)
        {
            role = await userRepository.GetRoleByIdAsync(
                request.RoleId!.Value,
                cancellationToken);
            if (role is null || role.Name == RoleNames.SuperAdmin)
            {
                throw new PsyLink.Application.Common.Exceptions.BusinessRuleException(
                    "Debe asignarse un rol válido distinto de SUPER_ADMIN.");
            }

            user.RoleId = role.RoleId;
        }

        if (request.ActiveSpecified)
        {
            user.Active = request.Active!.Value;
        }


        return new UserResponseDto
        {
            UserId = user.UserId,
            TenantId = user.TenantId!.Value,
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
            Role = new RoleResponseDto
            {
                RoleId = role.RoleId,
                Name = role.Name
            }
        };
    }
}
