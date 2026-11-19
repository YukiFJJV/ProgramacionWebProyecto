using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.Medications.Services.Queries;
using PsyLink.API.Authorization;

using PsyLink.Application.Features.Prescriptions.DTOs;
namespace PsyLink.API.Controllers;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("active-ingredients")]
[AllowAnonymous]
public sealed class ActiveIngredientsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List active ingredients")]
    [EndpointDescription("Returns registered active ingredients available for medication associations.")]
    [ProducesResponseType<IReadOnlyList<IngredientResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<IngredientResponseDto>>> Get(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetActiveIngredientsQuery(), cancellationToken);
        return Ok(result);
    }
}
