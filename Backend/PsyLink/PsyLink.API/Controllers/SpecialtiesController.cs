using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.Specialties.Services.Commands;
using PsyLink.Application.Features.Specialties.Services.Queries;
using PsyLink.API.Authorization;

using PsyLink.Application.Features.Specialties.DTOs;
namespace PsyLink.API.Controllers;

[ApiController]
[Authorize]
[Route("specialties")]
public sealed class SpecialtiesController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [EndpointSummary("Create a specialty")]
    [EndpointDescription("Creates a specialty with a unique name.")]
    [ProducesResponseType<SpecialtyResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SpecialtyResponseDto>> Create(
        [FromBody] SpecialtyRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateSpecialtyCommand(request.SpecialtyName),
            cancellationToken);
        return CreatedAtAction(nameof(Get), null, result);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin + "," + Roles.Psychologist + "," + Roles.Patient)]
    [EndpointSummary("List specialties")]
    [EndpointDescription("Returns the complete specialty catalog.")]
    [ProducesResponseType<IReadOnlyList<SpecialtyResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<SpecialtyResponseDto>>> Get(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSpecialtiesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{specialtyId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [EndpointSummary("Update a specialty")]
    [EndpointDescription("Changes a specialty name while preventing duplicate names.")]
    [ProducesResponseType<SpecialtyResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SpecialtyResponseDto>> Update(
        Guid specialtyId,
        [FromBody] SpecialtyRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateSpecialtyCommand(specialtyId, request.SpecialtyName),
            cancellationToken);
        return Ok(result);
    }
}
