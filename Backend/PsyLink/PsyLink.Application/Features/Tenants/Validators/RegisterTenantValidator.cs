using FluentValidation;
using PsyLink.Application.Features.Tenants.Services.Commands;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Tenants.Validators;

public sealed class RegisterTenantValidator : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantValidator(ITenantRepository tenantRepository)
    {
        RuleFor(command => command.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100)
            .MustAsync(async (name, cancellationToken) =>
                !await tenantRepository.ExistsByNameAsync(name, cancellationToken: cancellationToken))
            .WithMessage("Ya existe una empresa con ese nombre.");

        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(command => command.Phone)
            .NotEmpty()
            .Matches(@"^\d{8}$")
            .WithMessage("El teléfono debe contener exactamente 8 dígitos.");

        RuleFor(command => command.Rtn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (rtn, cancellationToken) =>
                !await tenantRepository.ExistsByRtnAsync(rtn, cancellationToken: cancellationToken))
            .WithMessage("Ya existe una empresa con ese RTN.");
    }
}
