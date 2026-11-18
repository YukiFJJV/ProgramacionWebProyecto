using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.API.DTOs;
using PsyLink.Application.Features.PatientRecords.Services.Queries;
using PsyLink.Application.Features.Diagnoses.Services.Commands;
using PsyLink.Application.Features.Diagnoses.Services.Queries;
using PsyLink.API.Authorization;
using AppPatientCreateRequestDto = PsyLink.Application.Features.Patients.DTOs.PatientCreateRequestDto;
using AppPatientResponseDto = PsyLink.Application.Features.Patients.DTOs.PatientResponseDto;
using AppPatientDiagnosisResponseDto = PsyLink.Application.Features.Diagnoses.DTOs.PatientDiagnosisResponseDto;
using AppUserCreateRequestDto = PsyLink.Application.Features.Users.DTOs.UserCreateRequestDto;
using AppUserResponseDto = PsyLink.Application.Features.Users.DTOs.UserResponseDto;

namespace PsyLink.API.Controllers;

[ApiController]
[Authorize]
[Route("patients")]
public sealed class PatientsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [EndpointSummary("Register a patient in the authenticated tenant")]
    [EndpointDescription("Creates a PATIENT account and its profile in one operation.")]
    [ProducesResponseType<PatientResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PatientResponseDto>> Create(
        [FromBody] PatientCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RegisterPatientCommand(new AppPatientCreateRequestDto
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
                BirthDate = request.BirthDate,
                Description = request.Description
            }),
            cancellationToken);

        var response = ToResponse(result);
        return CreatedAtAction(
            nameof(GetById),
            new { patientId = response.PatientId },
            response);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin + "," + Roles.Psychologist)]
    [EndpointSummary("List patients accessible to the current user")]
    [EndpointDescription("Administrators see tenant patients; psychologists see only patients linked to their appointments.")]
    [ProducesResponseType<PageResponseDto<PatientResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PageResponseDto<PatientResponseDto>>> Get(
        [FromQuery] PatientQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetPatientsQuery(
                query.Search,
                query.PsychologistId,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.SortDirection),
            cancellationToken);

        var response = new PageResponseDto<PatientResponseDto>(
            result.Items.Select(ToResponse).ToList(),
            result.Page,
            result.PageSize,
            result.TotalItems,
            result.TotalPages,
            result.HasPrevious,
            result.HasNext);

        return Ok(response);
    }

    [HttpGet("{patientId:guid}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Psychologist)]
    [EndpointSummary("Get a patient profile by ID")]
    [EndpointDescription("Returns a tenant patient profile; psychologists can access only patients linked to their appointments.")]
    [ProducesResponseType<PatientResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientResponseDto>> GetById(
        Guid patientId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPatientByIdQuery(patientId), cancellationToken);
        return Ok(ToResponse(result));
    }

    [HttpGet("{patientId:guid}/record")]
    [Authorize(Roles = Roles.Psychologist)]
    [EndpointSummary("Get an authorized patient's clinical record")]
    [EndpointDescription("Returns a patient's clinical record only when the authenticated psychologist has an active care relationship with the patient.")]
    [ProducesResponseType<RecordDetailResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecordDetailResponseDto>> GetRecord(
        Guid patientId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPatientRecordQuery(patientId), cancellationToken);
        return Ok(RecordDetailResponseMapper.ToResponse(result));
    }

    [HttpPut("{patientId:guid}")]
    [Authorize(Roles = Roles.Psychologist)]
    [EndpointSummary("Update an authorized patient profile")]
    [EndpointDescription("Updates the permitted profile fields for a patient linked to the authenticated psychologist.")]
    [ProducesResponseType<PatientResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PatientResponseDto>> Update(
        Guid patientId,
        [FromBody] PatientUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdatePatientProfileCommand(
                patientId,
                request.FirstName,
                request.FirstNameSpecified,
                request.LastName,
                request.LastNameSpecified,
                request.Email,
                request.EmailSpecified,
                request.PhoneNumber,
                request.PhoneNumberSpecified,
                request.BirthDate,
                request.BirthDateSpecified,
                request.Description,
                request.DescriptionSpecified),
            cancellationToken);

        return Ok(ToResponse(result));
    }

    [HttpGet("{patientId:guid}/diagnoses")]
    [Authorize(Roles = Roles.Psychologist)]
    [EndpointSummary("List diagnoses for an authorized patient")]
    [EndpointDescription("Returns diagnosis statuses associated with a patient the authenticated psychologist may access.")]
    [ProducesResponseType<IReadOnlyList<PatientDiagnosisResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PatientDiagnosisResponseDto>>> GetDiagnoses(
        Guid patientId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetPatientDiagnosesQuery(patientId),
            cancellationToken);
        return Ok(result.Select(ToResponse).ToList());
    }

    [HttpPut("{patientId:guid}/diagnoses/{diagnosisId:guid}")]
    [Authorize(Roles = Roles.Psychologist)]
    [EndpointSummary("Set a patient's diagnosis status")]
    [EndpointDescription("Creates or updates the diagnosis status for a patient linked to the authenticated psychologist.")]
    [ProducesResponseType<PatientDiagnosisResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDiagnosisResponseDto>> UpsertDiagnosis(
        Guid patientId,
        Guid diagnosisId,
        [FromBody] PatientDiagnosisUpsertRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpsertPatientDiagnosisCommand(
                patientId,
                diagnosisId,
                request.StatusId,
                request.Notes),
            cancellationToken);
        return Ok(ToResponse(result));
    }

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

    private static PatientDiagnosisResponseDto ToResponse(
        AppPatientDiagnosisResponseDto patientDiagnosis) =>
        new(
            patientDiagnosis.PatientId,
            new DiagnosisDetailResponseDto(
                patientDiagnosis.Diagnosis.DiagnosisId,
                patientDiagnosis.Diagnosis.Name,
                patientDiagnosis.Diagnosis.Description),
            new DiagnosisStatusResponseDto(
                patientDiagnosis.Status.StatusId,
                patientDiagnosis.Status.Name),
            patientDiagnosis.UpdatedAt);
}
