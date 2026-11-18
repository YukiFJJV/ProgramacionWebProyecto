using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.API.DTOs;
using PsyLink.Application.Features.Users.Services.Queries;
using PsyLink.API.Authorization;

namespace PsyLink.API.Controllers;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("roles")]
public sealed class RolesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List roles assignable by an administrator")]
    [EndpointDescription("Returns assignable roles for the tenant administrator, excluding SUPER_ADMIN.")]
    [ProducesResponseType<IReadOnlyList<RoleResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<RoleResponseDto>>> Get(
        CancellationToken cancellationToken)
    {
        var roles = await mediator.Send(new GetAssignableRolesQuery(), cancellationToken);
        return Ok(roles.Select(role => new RoleResponseDto(role.RoleId, role.Name)).ToList());
    }
}
