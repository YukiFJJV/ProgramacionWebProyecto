using FluentValidation;
using PsyLink.Application.Features.Tenants.Services.Commands;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Tenants.Validators;

public sealed class UpdateTenantValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantValidator(ITenantRepository tenantRepository)
    {
        RuleFor(command => command.TenantId).NotEmpty();
        RuleFor(command => command)
            .Must(command =>
                command.Name is not null ||
                command.EmailSpecified ||
                command.RtnSpecified ||
                command.PhoneSpecified ||
                command.Active.HasValue)
            .WithMessage("Debe especificar al menos un campo para actualizar.");

        When(command => command.Name is not null, () =>
        {
            RuleFor(command => command.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(100)
                .MustAsync(async (command, name, cancellationToken) =>
                    !await tenantRepository.ExistsByNameAsync(name!, command.TenantId, cancellationToken))
                .WithMessage("Ya existe una empresa con ese nombre.");
        });

        When(command => command.EmailSpecified && command.Email is not null, () =>
        {
            RuleFor(command => command.Email)
                .NotEmpty()
                .EmailAddress();
        });

        When(command => command.RtnSpecified && command.Rtn is not null, () =>
        {
            RuleFor(command => command.Rtn)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (command, rtn, cancellationToken) =>
                    !await tenantRepository.ExistsByRtnAsync(rtn!, command.TenantId, cancellationToken))
                .WithMessage("Ya existe una empresa con ese RTN.");
        });

        When(command => command.PhoneSpecified && command.Phone is not null, () =>
        {
            RuleFor(command => command.Phone)
                .NotEmpty()
                .Matches(@"^\d{8}$")
                .WithMessage("El teléfono debe contener exactamente 8 dígitos.");
        });
    }
}
