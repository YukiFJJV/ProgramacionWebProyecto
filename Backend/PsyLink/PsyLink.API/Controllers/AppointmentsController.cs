using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.API.DTOs;
using PsyLink.Application.Features.Appointments.Services.Commands;
using PsyLink.Application.Features.Appointments.Services.Queries;
using PsyLink.API.Authorization;
using AppAppointmentCreateRequestDto = PsyLink.Application.Features.Appointments.DTOs.AppointmentCreateRequestDto;
using AppAppointmentDetailResponseDto = PsyLink.Application.Features.Appointments.DTOs.AppointmentDetailResponseDto;
using AppAppointmentQueryDto = PsyLink.Application.Features.Appointments.DTOs.AppointmentQueryDto;
using AppAppointmentResponseDto = PsyLink.Application.Features.Appointments.DTOs.AppointmentResponseDto;
using AppAppointmentUpdateRequestDto = PsyLink.Application.Features.Appointments.DTOs.AppointmentUpdateRequestDto;
using AppPatientResponseDto = PsyLink.Application.Features.Patients.DTOs.PatientResponseDto;
using AppPsychologistSummaryResponseDto = PsyLink.Application.Features.Psychologists.DTOs.PsychologistSummaryResponseDto;
using AppUserResponseDto = PsyLink.Application.Features.Users.DTOs.UserResponseDto;

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
            new CreateAppointmentCommand(new AppAppointmentCreateRequestDto
            {
                PsychologistId = request.PsychologistId,
                PatientId = request.PatientId,
                DateAt = request.DateAt,
                StartsAt = request.StartsAt,
                EndsAt = request.EndsAt
            }),
            cancellationToken);
        var response = ToResponse(result);
        return CreatedAtAction(
            nameof(GetById),
            new { appointmentId = response.AppointmentId },
            response);
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
            new GetPsychologistAppointmentsQuery(new AppAppointmentQueryDto
            {
                PatientId = query.PatientId,
                PsychologistId = query.PsychologistId,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                Page = query.Page,
                PageSize = query.PageSize,
                SortDirection = query.SortDirection
            }),
            cancellationToken);

        return Ok(new PageResponseDto<AppointmentResponseDto>(
            result.Items.Select(ToResponse).ToList(),
            result.Page,
            result.PageSize,
            result.TotalItems,
            result.TotalPages,
            result.HasPrevious,
            result.HasNext));
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
        return Ok(ToResponse(result));
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
            new RescheduleAppointmentCommand(
                appointmentId,
                new AppAppointmentUpdateRequestDto
                {
                    DateAt = request.DateAt,
                    StartsAt = request.StartsAt,
                    EndsAt = request.EndsAt
                }),
            cancellationToken);
        return Ok(ToResponse(result));
    }

    private static AppointmentResponseDto ToResponse(AppAppointmentResponseDto appointment) =>
        new(
            appointment.AppointmentId,
            appointment.PatientId,
            appointment.PatientName,
            appointment.PsychologistId,
            appointment.PsychologistName,
            appointment.DateAt,
            appointment.StartsAt,
            appointment.EndsAt);

    private static AppointmentDetailResponseDto ToResponse(
        AppAppointmentDetailResponseDto appointment) =>
        new(
            appointment.AppointmentId,
            ToResponse(appointment.Patient),
            ToResponse(appointment.Psychologist),
            appointment.DateAt,
            appointment.StartsAt,
            appointment.EndsAt);

    private static PatientResponseDto ToResponse(AppPatientResponseDto patient) =>
        new(
            patient.PatientId,
            ToResponse(patient.User),
            patient.BirthDate,
            patient.Description,
            patient.Age);

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
}
