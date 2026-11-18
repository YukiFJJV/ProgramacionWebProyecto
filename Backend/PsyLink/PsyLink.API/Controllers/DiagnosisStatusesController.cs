using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.Diagnoses.Services.Queries;
using PsyLink.API.Authorization;

using PsyLink.Application.Features.Diagnoses.DTOs;
namespace PsyLink.API.Controllers;

[ApiController]
[Authorize(Roles = Roles.Psychologist)]
[Route("diagnosis-statuses")]
public sealed class DiagnosisStatusesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List patient diagnosis statuses")]
    [EndpointDescription("Returns the statuses available when recording a diagnosis for an authorized patient.")]
    [ProducesResponseType<IReadOnlyList<DiagnosisStatusResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<DiagnosisStatusResponseDto>>> Get(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetDiagnosisStatusesQuery(), cancellationToken);
        return Ok(result);
    }
}
