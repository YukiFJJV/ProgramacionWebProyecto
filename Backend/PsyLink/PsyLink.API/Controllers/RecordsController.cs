using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.PatientRecords.DTOs;
using PsyLink.Application.Features.Diagnoses.DTOs;
using PsyLink.Application.Features.Prescriptions.DTOs;
using PsyLink.Application.Features.Diagnoses.Services.Commands;
using PsyLink.Application.Features.PatientRecords.Services.Commands;
using PsyLink.Application.Features.PatientRecords.Services.Queries;
using PsyLink.Application.Features.Prescriptions.Services.Commands;
using PsyLink.API.Authorization;

namespace PsyLink.API.Controllers;

[ApiController]
[Authorize(Roles = Roles.Psychologist)]
[Route("records")]
public sealed class RecordsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a patient's clinical record")]
    [EndpointDescription("Creates the initial record for a patient with an active care relationship to the authenticated psychologist.")]
    [ProducesResponseType<RecordDetailResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RecordDetailResponseDto>> Create(
        [FromBody] RecordCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreatePatientRecordCommand(request),
            cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { recordId = result.RecordId },
            result);
    }

    [HttpGet("{recordId:guid}")]
    [EndpointSummary("Get a clinical record by ID")]
    [EndpointDescription("Returns a record only when it belongs to the psychologist's tenant and the psychologist has an active care relationship with its patient.")]
    [ProducesResponseType<RecordDetailResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecordDetailResponseDto>> GetById(
        Guid recordId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRecordByIdQuery(recordId), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{recordId:guid}")]
    [EndpointSummary("Update a clinical record")]
    [EndpointDescription("Partially updates the allowed clinical record fields for an authorized patient.")]
    [ProducesResponseType<RecordDetailResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RecordDetailResponseDto>> Update(
        Guid recordId,
        [FromBody] RecordUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdatePatientRecordCommand(recordId, request),
            cancellationToken);
        return Ok(result);
    }

    [HttpPut("{recordId:guid}/diagnoses/{diagnosisId:guid}")]
    [EndpointSummary("Associate or update a diagnosis on a record")]
    [EndpointDescription("Creates or updates the notes for a diagnosis associated with an authorized clinical record.")]
    [ProducesResponseType<RecordDiagnosisResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RecordDiagnosisResponseDto>> UpsertDiagnosis(
        Guid recordId,
        Guid diagnosisId,
        [FromBody] RecordDiagnosisRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpsertRecordDiagnosisCommand(recordId, diagnosisId, request),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{recordId:guid}/diagnoses/{diagnosisId:guid}")]
    [EndpointSummary("Remove a diagnosis from a clinical record")]
    [EndpointDescription("Removes the diagnosis association after verifying the record is accessible to the authenticated psychologist.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDiagnosis(
        Guid recordId,
        Guid diagnosisId,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new DeleteRecordDiagnosisCommand(recordId, diagnosisId),
            cancellationToken);
        return NoContent();
    }

    [HttpPost("{recordId:guid}/prescriptions")]
    [EndpointSummary("Create a prescription on a clinical record")]
    [EndpointDescription("Adds a prescription for an existing medication to a record the authenticated psychologist may access.")]
    [ProducesResponseType<PrescriptionResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PrescriptionResponseDto>> CreatePrescription(
        Guid recordId,
        [FromBody] PrescriptionRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreatePrescriptionCommand(recordId, request),
            cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { recordId = result.RecordId },
            result);
    }

    [HttpPut("{recordId:guid}/prescriptions/{prescriptionId:guid}")]
    [EndpointSummary("Update a prescription on a clinical record")]
    [EndpointDescription("Updates the medication, dosage, or quantity for a prescription belonging to the specified accessible record.")]
    [ProducesResponseType<PrescriptionResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PrescriptionResponseDto>> UpdatePrescription(
        Guid recordId,
        Guid prescriptionId,
        [FromBody] PrescriptionUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdatePrescriptionCommand(recordId, prescriptionId, request),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{recordId:guid}/prescriptions/{prescriptionId:guid}")]
    [EndpointSummary("Delete a prescription from a clinical record")]
    [EndpointDescription("Removes a prescription belonging to the specified accessible clinical record.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePrescription(
        Guid recordId,
        Guid prescriptionId,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new DeletePrescriptionCommand(recordId, prescriptionId),
            cancellationToken);
        return NoContent();
    }
}
