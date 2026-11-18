using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.Schedules.Services.Commands;
using PsyLink.API.Authorization;

using PsyLink.Application.Features.Schedules.DTOs;
namespace PsyLink.API.Controllers;

[ApiController]
[Authorize]
[Route("schedules")]
public sealed class SchedulesController(ISender mediator) : ControllerBase
{
    [HttpPut("{scheduleId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [EndpointSummary("Update a weekly schedule block")]
    [EndpointDescription("Updates a schedule block belonging to the authenticated tenant, rejecting overlapping times.")]
    [ProducesResponseType<ScheduleResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScheduleResponseDto>> Update(
        Guid scheduleId,
        [FromBody] ScheduleRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateWorkScheduleCommand(scheduleId, request),
            cancellationToken);
        return Ok(result);
    }
}
