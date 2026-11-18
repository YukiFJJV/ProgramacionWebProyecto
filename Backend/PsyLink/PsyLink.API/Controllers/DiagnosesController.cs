using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.API.DTOs;
using PsyLink.Application.Features.Diagnoses.Services.Commands;
using PsyLink.Application.Features.Diagnoses.Services.Queries;
using PsyLink.API.Authorization;
using AppDiagnosisDetailResponseDto = PsyLink.Application.Features.Diagnoses.DTOs.DiagnosisDetailResponseDto;
using AppDiagnosisQueryDto = PsyLink.Application.Features.Diagnoses.DTOs.DiagnosisQueryDto;
using AppDiagnosisResponseDto = PsyLink.Application.Features.Diagnoses.DTOs.DiagnosisResponseDto;
using AppDiagnosisRequestDto = PsyLink.Application.Features.Diagnoses.DTOs.DiagnosisRequestDto;

namespace PsyLink.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Route("diagnoses")]
public sealed class DiagnosesController(IMediator mediator) : ControllerBase
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
        var result = await mediator.Send(
            new GetDiagnosesQuery(new AppDiagnosisQueryDto
            {
                Search = query.Search,
                Page = query.Page,
                PageSize = query.PageSize,
                SortDirection = query.SortDirection
            }),
            cancellationToken);

        return Ok(new PageResponseDto<DiagnosisResponseDto>(
            result.Items.Select(ToResponse).ToList(),
            result.Page,
            result.PageSize,
            result.TotalItems,
            result.TotalPages,
            result.HasPrevious,
            result.HasNext));
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
        return Ok(ToResponse(result));
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
        var result = await mediator.Send(
            new CreateDiagnosisCommand(new AppDiagnosisRequestDto
            {
                Name = request.Name,
                Description = request.Description
            }),
            cancellationToken);
        var response = ToResponse(result);
        return CreatedAtAction(nameof(GetById), new { diagnosisId = response.DiagnosisId }, response);
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
            new UpdateDiagnosisCommand(
                diagnosisId,
                new AppDiagnosisRequestDto
                {
                    Name = request.Name,
                    Description = request.Description
                }),
            cancellationToken);
        return Ok(ToResponse(result));
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

    private static DiagnosisResponseDto ToResponse(AppDiagnosisResponseDto diagnosis) =>
        new(diagnosis.DiagnosisId, diagnosis.Name);

    private static DiagnosisDetailResponseDto ToResponse(
        AppDiagnosisDetailResponseDto diagnosis) =>
        new(diagnosis.DiagnosisId, diagnosis.Name, diagnosis.Description);
}
