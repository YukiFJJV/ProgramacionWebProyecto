using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.Medications.Services.Commands;
using PsyLink.Application.Features.Medications.Services.Queries;
using PsyLink.API.Authorization;

using PsyLink.Application.Features.Medications.DTOs;
using PsyLink.Application.Features.Prescriptions.DTOs;
using PsyLink.Application.Features.Shared.DTOs;
namespace PsyLink.API.Controllers;

[ApiController]
[Authorize]
[Route("medications")]
public sealed class MedicationsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = Roles.Admin + "," + Roles.Psychologist)]
    [EndpointSummary("List medications in the catalog")]
    [EndpointDescription("Searches medications by name, laboratory, or active ingredient and returns a paginated result.")]
    [ProducesResponseType<PageResponseDto<MedicationResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PageResponseDto<MedicationResponseDto>>> Get(
        [FromQuery] MedicationQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMedicationsQuery(query), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [EndpointSummary("Create a medication")]
    [EndpointDescription("Creates a medication and links it to existing active ingredients and an optional laboratory.")]
    [ProducesResponseType<MedicationResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicationResponseDto>> Create(
        [FromBody] PsyLink.Application.Features.Medications.DTOs.MedicationCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateMedicationCommand(request), cancellationToken);
        return CreatedAtAction(nameof(Get), null, result);
    }
}
