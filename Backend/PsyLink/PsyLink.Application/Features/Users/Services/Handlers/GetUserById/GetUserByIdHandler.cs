using MediatR;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.Users.Services.Handlers.CreateUser;
using PsyLink.Application.Features.Users.Services.Handlers.GetUsers;
using PsyLink.Application.Features.Users.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Users.Services.Handlers.GetUserById;

public sealed class GetUserByIdHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetUserByIdQuery, UserResponseDto>
{
    public async Task<UserResponseDto> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = GetUsersHandler.EnsureCompanyAdmin(currentUser);
        var user = await userRepository.GetByIdForTenantAsync(
            request.UserId,
            tenantId,
            cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el usuario {request.UserId}.");

        return CreateUserHandler.ToResponse(user);
    }
}
