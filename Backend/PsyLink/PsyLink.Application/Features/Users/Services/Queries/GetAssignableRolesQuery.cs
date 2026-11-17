using MediatR;
using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Users.Services.Queries;

public record GetAssignableRolesQuery : IRequest<IReadOnlyList<RoleResponseDto>>;
