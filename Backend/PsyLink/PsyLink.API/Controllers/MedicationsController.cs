using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.API.DTOs;
using PsyLink.Application.Features.Medications.Services.Commands;
using PsyLink.Application.Features.Medications.Services.Queries;
using PsyLink.API.Authorization;
using AppMedicationCreateRequestDto = PsyLink.Application.Features.Medications.DTOs.MedicationCreateRequestDto;
using AppMedicationQueryDto = PsyLink.Application.Features.Medications.DTOs.MedicationQueryDto;
using AppMedicationResponseDto = PsyLink.Application.Features.Prescriptions.DTOs.MedicationResponseDto;

namespace PsyLink.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Route("medications")]
public sealed class MedicationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = Roles.Admin + "," + Roles.Psychologist)]
    [EndpointSummary("List medications in the catalog")]
    [EndpointDescription("Searches medications by name, laboratory, or active ingredient and returns a paginated result.")]
    [ProducesResponseType<PageResponseDto<MedicationResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PageResponseDto<MedicationResponseDto>>> Get(
        [FromQuery] MedicationQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetMedicationsQuery(new AppMedicationQueryDto
            {
                Search = query.Search,
                LaboratoryId = query.LaboratoryId,
                IngredientId = query.IngredientId,
                Page = query.Page,
                PageSize = query.PageSize
            }),
            cancellationToken);

        return Ok(new PageResponseDto<MedicationResponseDto>(
            result.Items.Select(ToResponse).ToList(),
            result.Page,
            result.PageSize,
            result.TotalItems,
            result.TotalPages,
            result.HasPrevious,
            result.HasNext));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [EndpointSummary("Create a medication")]
    [EndpointDescription("Creates a medication and links it to existing active ingredients and an optional laboratory.")]
    [ProducesResponseType<MedicationResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicationResponseDto>> Create(
        [FromBody] PsyLink.API.DTOs.MedicationCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new CreateMedicationCommand(new AppMedicationCreateRequestDto
            {
                LaboratoryId = request.LaboratoryId,
                Name = request.Name,
                Content = request.Content,
                Presentation = request.Presentation,
                IngredientIds = request.IngredientIds ?? new List<Guid>()
            }),
            cancellationToken);
        var response = ToResponse(result);
        return Created($"/medications/{response.MedicationId}", response);
    }

    private static MedicationResponseDto ToResponse(AppMedicationResponseDto medication) =>
        new(
            medication.MedicationId,
            medication.Name,
            medication.Content,
            medication.Presentation,
            medication.Laboratory is null
                ? null
                : new LaboratoryResponseDto(
                    medication.Laboratory.LaboratoryId,
                    medication.Laboratory.Name,
                    medication.Laboratory.Phone is null
                        ? null
                        : new PhoneNumberResponseDto(
                            medication.Laboratory.Phone.PhoneId,
                            medication.Laboratory.Phone.Number)),
            medication.Ingredients
                .Select(ingredient => new IngredientResponseDto(
                    ingredient.IngredientId,
                    ingredient.Name))
                .ToList());
}
