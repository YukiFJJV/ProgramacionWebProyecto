using MediatR;
using PsyLink.Application.Features.Medications.Services.Queries;
using PsyLink.Application.Features.Prescriptions.DTOs;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Medications.Services.Handlers.GetActiveIngredients;

public sealed class GetActiveIngredientsHandler(
    IActiveIngredientRepository ingredientRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetActiveIngredientsQuery, IReadOnlyList<IngredientResponseDto>>
{
    public async Task<IReadOnlyList<IngredientResponseDto>> Handle(
        GetActiveIngredientsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.RoleName != RoleNames.Admin)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere el rol ADMIN.");
        }

        var ingredients = await ingredientRepository.GetAllAsync(cancellationToken);
        return ingredients.Select(ingredient => new IngredientResponseDto
        {
            IngredientId = ingredient.IngredientId,
            Name = ingredient.Name
        }).ToList();
    }
}
