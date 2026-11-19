using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.Medications.Services.Queries;
using PsyLink.API.Authorization;

using PsyLink.Application.Features.Prescriptions.DTOs;
namespace PsyLink.API.Controllers;

[ApiController]
// [Authorize(Roles = Roles.Admin)]
[Authorize]
[Route("laboratories")]
[AllowAnonymous]
public sealed class LaboratoriesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List laboratories")]
    [EndpointDescription("Returns registered laboratories with their optional phone numbers.")]
    [ProducesResponseType<IReadOnlyList<LaboratoryResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<LaboratoryResponseDto>>> Get(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetLaboratoriesQuery(), cancellationToken);
        return Ok(result);
    }
}
