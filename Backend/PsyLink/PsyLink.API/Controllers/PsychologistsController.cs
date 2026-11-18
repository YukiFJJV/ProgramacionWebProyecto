using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.Psychologists.Services.Commands;
using PsyLink.Application.Features.Psychologists.Services.Queries;
using PsyLink.Application.Features.Schedules.Services.Commands;
using PsyLink.Application.Features.Schedules.Services.Queries;
using PsyLink.API.Authorization;

using PsyLink.Application.Features.Psychologists.DTOs;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.Schedules.DTOs;
using PsyLink.Application.Features.Shared.DTOs;
namespace PsyLink.API.Controllers;

[ApiController]
[Authorize]
[Route("psychologists")]
public sealed class PsychologistsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [EndpointSummary("Register a psychologist in the authenticated tenant")]
    [EndpointDescription("Creates a PSYCHOLOGIST account and professional profile with a validated specialty and license.")]
    [ProducesResponseType<PsychologistDetailResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PsychologistDetailResponseDto>> Create(
        [FromBody] PsychologistCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RegisterPsychologistCommand(request),
            cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { psychologistId = result.PsychologistId },
            result);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin + "," + Roles.Patient + "," + Roles.Psychologist)]
    [EndpointSummary("List psychologists in the authenticated tenant")]
    [EndpointDescription("Returns a paged psychologist list with search, specialty, availability, and sorting filters.")]
    [ProducesResponseType<PageResponseDto<PsychologistSummaryResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PageResponseDto<PsychologistSummaryResponseDto>>> Get(
        [FromQuery] PsychologistQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetPsychologistsQuery(
                query.Search,
                query.SpecialtyId,
                query.AvailableOn,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.SortDirection),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{psychologistId:guid}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Patient + "," + Roles.Psychologist)]
    [EndpointSummary("Get a psychologist profile")]
    [EndpointDescription("Returns the professional profile, specialty, and weekly schedules for an accessible tenant psychologist.")]
    [ProducesResponseType<PsychologistDetailResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PsychologistDetailResponseDto>> GetById(
        Guid psychologistId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetPsychologistProfileQuery(psychologistId),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{psychologistId:guid}/schedules")]
    [Authorize(Roles = Roles.Admin)]
    [EndpointSummary("Add a psychologist's weekly schedule block")]
    [EndpointDescription("Adds a non-overlapping work schedule block to a psychologist in the authenticated tenant.")]
    [ProducesResponseType<ScheduleResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScheduleResponseDto>> CreateSchedule(
        Guid psychologistId,
        [FromBody] ScheduleRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateWorkScheduleCommand(psychologistId, request),
            cancellationToken);
        return CreatedAtAction(
            nameof(GetSchedules),
            new { psychologistId = result.PsychologistId },
            result);
    }

    [HttpGet("{psychologistId:guid}/schedules")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Patient)]
    [EndpointSummary("List a psychologist's weekly schedule")]
    [EndpointDescription("Returns the weekly schedule blocks for a psychologist in the authenticated tenant.")]
    [ProducesResponseType<IReadOnlyList<ScheduleResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ScheduleResponseDto>>> GetSchedules(
        Guid psychologistId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetWorkSchedulesQuery(psychologistId),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{psychologistId:guid}/availability")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Patient + "," + Roles.Psychologist)]
    [EndpointSummary("Get psychologist availability")]
    [EndpointDescription("Calculates available appointment slots from published work schedules and existing appointments.")]
    [ProducesResponseType<IReadOnlyList<AvailabilityResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<AvailabilityResponseDto>>> GetAvailability(
        Guid psychologistId,
        [FromQuery] AvailabilityQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetPsychologistAvailabilityQuery(psychologistId, query),
            cancellationToken);
        return Ok(result);
    }
}
