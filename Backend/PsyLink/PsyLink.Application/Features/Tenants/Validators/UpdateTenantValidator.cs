using FluentValidation;
using PsyLink.Application.Features.Tenants.Services.Commands;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Tenants.Validators;

public sealed class UpdateTenantValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantValidator(ITenantRepository tenantRepository)
    {
        RuleFor(command => command.TenantId).NotEmpty();
        RuleFor(command => command.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100)
            .MustAsync(async (command, name, cancellationToken) =>
                !await tenantRepository.ExistsByNameAsync(name, command.TenantId, cancellationToken))
            .WithMessage("Ya existe una empresa con ese nombre.");
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(command => command.Rtn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (command, rtn, cancellationToken) =>
                !await tenantRepository.ExistsByRtnAsync(rtn, command.TenantId, cancellationToken))
            .WithMessage("Ya existe una empresa con ese RTN.");
        RuleFor(command => command.Phone)
            .NotEmpty()
            .Matches(@"^\d{8}$")
            .WithMessage("El teléfono debe contener exactamente 8 dígitos.");
    }
}
