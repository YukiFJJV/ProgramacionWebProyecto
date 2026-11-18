using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.API.DTOs;
using PsyLink.Application.Features.Psychologists.Services.Commands;
using PsyLink.Application.Features.Psychologists.Services.Queries;
using PsyLink.Application.Features.Schedules.Services.Commands;
using PsyLink.Application.Features.Schedules.Services.Queries;
using PsyLink.API.Authorization;
using AppPsychologistCreateRequestDto = PsyLink.Application.Features.Psychologists.DTOs.PsychologistCreateRequestDto;
using AppPsychologistDetailResponseDto = PsyLink.Application.Features.Psychologists.DTOs.PsychologistDetailResponseDto;
using AppPsychologistSummaryResponseDto = PsyLink.Application.Features.Psychologists.DTOs.PsychologistSummaryResponseDto;
using AppAvailabilityQueryDto = PsyLink.Application.Features.Psychologists.DTOs.AvailabilityQueryDto;
using AppUserCreateRequestDto = PsyLink.Application.Features.Users.DTOs.UserCreateRequestDto;
using AppUserResponseDto = PsyLink.Application.Features.Users.DTOs.UserResponseDto;
using AppScheduleRequestDto = PsyLink.Application.Features.Schedules.DTOs.ScheduleRequestDto;
using AppScheduleResponseDto = PsyLink.Application.Features.Schedules.DTOs.WorkScheduleResponseDto;

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
            new RegisterPsychologistCommand(new AppPsychologistCreateRequestDto
            {
                User = new AppUserCreateRequestDto
                {
                    Email = request.User.Email,
                    Password = request.User.Password,
                    FirstName = request.User.FirstName,
                    LastName = request.User.LastName,
                    PhoneNumber = request.User.PhoneNumber,
                    RoleId = request.User.RoleId
                },
                SpecialtyId = request.SpecialtyId,
                LicenseNumber = request.LicenseNumber,
                AdminId = request.AdminId
            }),
            cancellationToken);

        var response = ToResponse(result);
        return CreatedAtAction(
            nameof(GetById),
            new { psychologistId = response.PsychologistId },
            response);
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

        var response = new PageResponseDto<PsychologistSummaryResponseDto>(
            result.Items.Select(ToResponse).ToList(),
            result.Page,
            result.PageSize,
            result.TotalItems,
            result.TotalPages,
            result.HasPrevious,
            result.HasNext);

        return Ok(response);
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
        return Ok(ToResponse(result));
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
            new CreateWorkScheduleCommand(
                psychologistId,
                new AppScheduleRequestDto
                {
                    WeekDay = request.WeekDay,
                    StartsAt = request.StartsAt,
                    EndsAt = request.EndsAt
                }),
            cancellationToken);

        var response = ToResponse(result);
        return CreatedAtAction(
            nameof(GetSchedules),
            new { psychologistId = response.PsychologistId },
            response);
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
        return Ok(result.Select(ToResponse).ToList());
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
            new GetPsychologistAvailabilityQuery(
                psychologistId,
                new AppAvailabilityQueryDto
                {
                    DateFrom = query.DateFrom,
                    DateTo = query.DateTo,
                    DurationMinutes = query.DurationMinutes,
                    Timezone = query.Timezone
                }),
            cancellationToken);

        return Ok(result.Select(availability => new AvailabilityResponseDto(
            availability.PsychologistId,
            availability.Date,
            availability.Slots.Select(slot =>
                new AvailabilitySlotDto(slot.StartsAt, slot.EndsAt)).ToList())).ToList());
    }

    private static PsychologistSummaryResponseDto ToResponse(
        AppPsychologistSummaryResponseDto psychologist) =>
        new(
            psychologist.PsychologistId,
            psychologist.FirstName,
            psychologist.LastName,
            new SpecialtyResponseDto(
                psychologist.Specialty.SpecialtyId,
                psychologist.Specialty.SpecialtyName),
            psychologist.LicenseNumber);

    private static ScheduleResponseDto ToResponse(AppScheduleResponseDto schedule) =>
        new(
            schedule.ScheduleId,
            schedule.PsychologistId,
            schedule.WeekDay.ToString().ToUpperInvariant(),
            schedule.StartsAt,
            schedule.EndsAt);

    private static PsychologistDetailResponseDto ToResponse(
        AppPsychologistDetailResponseDto psychologist) =>
        new(
            psychologist.PsychologistId,
            ToResponse(psychologist.User),
            new SpecialtyResponseDto(
                psychologist.Specialty.SpecialtyId,
                psychologist.Specialty.SpecialtyName),
            psychologist.LicenseNumber,
            psychologist.Schedules.Select(schedule => new ScheduleResponseDto(
                schedule.ScheduleId,
                schedule.PsychologistId,
                schedule.WeekDay.ToString(),
                schedule.StartsAt,
                schedule.EndsAt)).ToList());

    private static UserResponseDto ToResponse(AppUserResponseDto user) =>
        new(
            user.UserId,
            user.TenantId,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Phone is null
                ? null
                : new PhoneNumberResponseDto(user.Phone.PhoneId, user.Phone.Number),
            new RoleResponseDto(user.Role.RoleId, user.Role.Name));
}