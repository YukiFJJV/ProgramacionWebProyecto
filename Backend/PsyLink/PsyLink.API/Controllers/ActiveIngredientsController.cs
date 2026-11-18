using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.API.DTOs;
using PsyLink.Application.Features.Medications.Services.Queries;
using PsyLink.API.Authorization;
using AppIngredientResponseDto = PsyLink.Application.Features.Prescriptions.DTOs.IngredientResponseDto;

namespace PsyLink.API.Controllers;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("api/active-ingredients")]
[Route("active-ingredients")]
public sealed class ActiveIngredientsController(IMediator mediator) : ControllerBase
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
        return Ok(result.Select(ToResponse).ToList());
    }

    private static IngredientResponseDto ToResponse(AppIngredientResponseDto ingredient) =>
        new(ingredient.IngredientId, ingredient.Name);
}
