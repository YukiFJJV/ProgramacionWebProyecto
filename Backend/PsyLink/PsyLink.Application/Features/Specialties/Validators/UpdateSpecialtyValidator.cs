using FluentValidation;
using PsyLink.Application.Features.Specialties.Services.Commands;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Specialties.Validators;

public sealed class UpdateSpecialtyValidator : AbstractValidator<UpdateSpecialtyCommand>
{
    public UpdateSpecialtyValidator(ISpecialtyRepository specialtyRepository)
    {
        RuleFor(command => command.SpecialtyId).NotEmpty();
        RuleFor(command => command.SpecialtyName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (command, name, cancellationToken) =>
                !await specialtyRepository.ExistsByNameAsync(name, command.SpecialtyId, cancellationToken))
            .WithMessage("Ya existe una especialidad con ese nombre.");
    }
}
