using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Medications.Services.Commands;
using PsyLink.Application.Features.Prescriptions.DTOs;
using PsyLink.Application.Features.Medications.Services;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;
using PsyLink.Domain.Pharmacy;

namespace PsyLink.Application.Features.Medications.Services.Handlers.CreateMedication;

public sealed class CreateMedicationHandler(
    IMedicationRepository medicationRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateMedicationCommand, MedicationResponseDto>
{
    public async Task<MedicationResponseDto> Handle(
        CreateMedicationCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.RoleName != RoleNames.Admin)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere el rol ADMIN.");
        }

        var input = request.Medication;
        if (input.LaboratoryId is Guid laboratoryId &&
            !await medicationRepository.LaboratoryExistsAsync(laboratoryId, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el laboratorio {laboratoryId}.");
        }

        var ingredientIds = input.IngredientIds?.Distinct().ToArray() ?? Array.Empty<Guid>();
        var ingredients = await medicationRepository.GetIngredientsByIdsAsync(
            ingredientIds,
            cancellationToken);
        if (ingredients.Count != ingredientIds.Length)
        {
            throw new PsyLink.Application.Common.Exceptions.BusinessRuleException("Uno o más principios activos no existen.");
        }

        var medication = new Medication
        {
            MedicationId = Guid.NewGuid(),
            LaboratoryId = input.LaboratoryId,
            Name = input.Name.Trim(),
            Content = input.Content,
            Presentation = input.Presentation.Trim()
        };
        medication.Ingredients = ingredientIds.Select(ingredientId => new MedicationIngredient
        {
            MedicationId = medication.MedicationId,
            IngredientId = ingredientId,
            Medication = medication
        }).ToList();

        await medicationRepository.AddAsync(medication, cancellationToken);
        var savedMedication = await medicationRepository.GetByIdAsync(
                medication.MedicationId,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.ConflictException("No se pudo recuperar el medicamento recién creado.");
        return MedicationDtoMapper.ToResponse(savedMedication);
    }
}
