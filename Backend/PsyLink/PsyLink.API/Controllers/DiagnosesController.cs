using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.Diagnoses.Services.Commands;
using PsyLink.Application.Features.Diagnoses.Services.Queries;
using PsyLink.API.Authorization;

using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.Shared.DTOs;
namespace PsyLink.API.Controllers;

[ApiController]
[Authorize]
[Route("diagnoses")]
public sealed class DiagnosesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = Roles.Admin + "," + Roles.Psychologist)]
    [EndpointSummary("List diagnoses in the catalog")]
    [EndpointDescription("Searches active catalog diagnoses by name and returns a paginated result.")]
    [ProducesResponseType<PageResponseDto<DiagnosisResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PageResponseDto<DiagnosisResponseDto>>> Get(
        [FromQuery] DiagnosisQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetDiagnosesQuery(query), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{diagnosisId:guid}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Psychologist)]
    [EndpointSummary("Get diagnosis details")]
    [EndpointDescription("Returns an active diagnosis catalog entry by its identifier.")]
    [ProducesResponseType<DiagnosisDetailResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DiagnosisDetailResponseDto>> GetById(
        Guid diagnosisId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetDiagnosisDetailQuery(diagnosisId),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [EndpointSummary("Create a diagnosis")]
    [EndpointDescription("Adds a diagnosis to the global catalog.")]
    [ProducesResponseType<DiagnosisDetailResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DiagnosisDetailResponseDto>> Create(
        [FromBody] DiagnosisRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateDiagnosisCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { diagnosisId = result.DiagnosisId }, result);
    }

    [HttpPut("{diagnosisId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [EndpointSummary("Update a diagnosis")]
    [EndpointDescription("Updates the name and description of an active catalog diagnosis.")]
    [ProducesResponseType<DiagnosisDetailResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DiagnosisDetailResponseDto>> Update(
        Guid diagnosisId,
        [FromBody] DiagnosisRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateDiagnosisCommand(diagnosisId, request),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{diagnosisId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [EndpointSummary("Delete a diagnosis")]
    [EndpointDescription("Deletes an unreferenced diagnosis or marks a referenced diagnosis inactive.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid diagnosisId,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteDiagnosisCommand(diagnosisId), cancellationToken);
        return NoContent();
    }
}
