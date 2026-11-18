using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Shared.DTOs;
using PsyLink.Application.Features.Appointments.Services.Commands;
using PsyLink.Application.Features.Appointments.Services.Queries;
using PsyLink.API.Authorization;

namespace PsyLink.API.Controllers;

[ApiController]
[Authorize]
[Route("appointments")]
public sealed class AppointmentsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Psychologist + "," + Roles.Patient)]
    [EndpointSummary("Create an appointment")]
    [EndpointDescription("Books an available slot for the authenticated patient, or for an eligible high-risk patient when booked by their psychologist.")]
    [ProducesResponseType<AppointmentResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentResponseDto>> Create(
        [FromBody] AppointmentCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateAppointmentCommand(request),
            cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { appointmentId = result.AppointmentId },
            result);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin + "," + Roles.Psychologist + "," + Roles.Patient)]
    [EndpointSummary("List appointments available to the current user")]
    [EndpointDescription("Returns tenant appointments filtered by patient, psychologist, date range, and pagination; results are scoped to the caller's role.")]
    [ProducesResponseType<PageResponseDto<AppointmentResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PageResponseDto<AppointmentResponseDto>>> Get(
        [FromQuery] AppointmentQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetPsychologistAppointmentsQuery(query),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{appointmentId:guid}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Psychologist + "," + Roles.Patient)]
    [EndpointSummary("Get an appointment's details")]
    [EndpointDescription("Returns the appointment and associated patient and psychologist, subject to tenant and caller scope.")]
    [ProducesResponseType<AppointmentDetailResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentDetailResponseDto>> GetById(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAppointmentDetailQuery(appointmentId),
            cancellationToken);
        return Ok(result);
    }

    [HttpPut("{appointmentId:guid}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Patient)]
    [EndpointSummary("Reschedule an appointment")]
    [EndpointDescription("Changes an appointment's date or time after checking tenant scope, work schedules, availability, and the patient's rescheduling limit.")]
    [ProducesResponseType<AppointmentResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentResponseDto>> Reschedule(
        Guid appointmentId,
        [FromBody] AppointmentUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RescheduleAppointmentCommand(appointmentId, request),
            cancellationToken);
        return Ok(result);
    }
}
