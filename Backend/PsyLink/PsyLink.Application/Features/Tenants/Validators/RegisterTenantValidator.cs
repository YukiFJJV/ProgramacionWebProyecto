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
            .EmailAddress()
            .When(command => !string.IsNullOrWhiteSpace(command.Email));

        RuleFor(command => command.PhoneNumber)
            .Matches(@"^\d{8}$")
            .When(command => !string.IsNullOrWhiteSpace(command.PhoneNumber))
            .WithMessage("El teléfono debe contener exactamente 8 dígitos.");

        RuleFor(command => command.Rtn)
            .MustAsync(async (rtn, cancellationToken) =>
                string.IsNullOrWhiteSpace(rtn) ||
                !await tenantRepository.ExistsByRtnAsync(rtn, cancellationToken: cancellationToken))
            .WithMessage("Ya existe una empresa con ese RTN.")
            .When(command => !string.IsNullOrWhiteSpace(command.Rtn));
    }
}
