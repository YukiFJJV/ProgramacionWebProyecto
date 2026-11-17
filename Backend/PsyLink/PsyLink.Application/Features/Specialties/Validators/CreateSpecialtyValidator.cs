using FluentValidation;
using PsyLink.Application.Features.Specialties.Services.Commands;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Specialties.Validators;

public sealed class CreateSpecialtyValidator : AbstractValidator<CreateSpecialtyCommand>
{
    public CreateSpecialtyValidator(ISpecialtyRepository specialtyRepository)
    {
        RuleFor(command => command.SpecialtyName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, cancellationToken) =>
                !await specialtyRepository.ExistsByNameAsync(name, cancellationToken: cancellationToken))
            .WithMessage("Ya existe una especialidad con ese nombre.");
    }
}
