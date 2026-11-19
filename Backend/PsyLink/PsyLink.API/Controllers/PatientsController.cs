using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.PatientRecords.Services.Queries;
using PsyLink.Application.Features.Diagnoses.Services.Commands;
using PsyLink.Application.Features.Diagnoses.Services.Queries;
using PsyLink.API.Authorization;

using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.PatientRecords.DTOs;
using PsyLink.Application.Features.Shared.DTOs;
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
            new RegisterPatientCommand(request),
            cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { patientId = result.PatientId },
            result);
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

        return Ok(result);
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
        return Ok(result);
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
        return Ok(result);
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

        return Ok(result);
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
        return Ok(result);
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
        return Ok(result);
    }
}
