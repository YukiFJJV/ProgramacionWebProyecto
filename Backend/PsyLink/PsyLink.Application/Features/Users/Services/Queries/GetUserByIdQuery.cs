using MediatR;
using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Users.Services.Queries;

public record GetUserByIdQuery(Guid UserId) : IQuery<UserResponseDto>;
