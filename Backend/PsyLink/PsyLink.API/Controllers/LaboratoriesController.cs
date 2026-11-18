using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.API.DTOs;
using PsyLink.Application.Features.Medications.Services.Queries;
using PsyLink.API.Authorization;
using AppLaboratoryResponseDto = PsyLink.Application.Features.Prescriptions.DTOs.LaboratoryResponseDto;

namespace PsyLink.API.Controllers;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("laboratories")]
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
        return Ok(result.Select(ToResponse).ToList());
    }

    private static LaboratoryResponseDto ToResponse(AppLaboratoryResponseDto laboratory) =>
        new(
            laboratory.LaboratoryId,
            laboratory.Name,
            laboratory.Phone is null
                ? null
                : new PhoneNumberResponseDto(
                    laboratory.Phone.PhoneId,
                    laboratory.Phone.Number));
}
