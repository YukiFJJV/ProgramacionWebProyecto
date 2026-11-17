using FluentValidation;
using PsyLink.Application.Features.Prescriptions.Services.Commands;

namespace PsyLink.Application.Features.Prescriptions.Validators;

public sealed class UpdatePrescriptionValidator : AbstractValidator<UpdatePrescriptionCommand>
{
    public UpdatePrescriptionValidator()
    {
        RuleFor(request => request.RecordId).NotEmpty();
        RuleFor(request => request.PrescriptionId).NotEmpty();
        RuleFor(request => request.Update).NotNull();
        When(request => request.Update is not null, () =>
        {
            RuleFor(request => request.Update.MedicationId)
                .Must(id => id is null || id != Guid.Empty)
                .WithMessage("El identificador del medicamento no puede estar vacío.");
            RuleFor(request => request.Update.Dosage)
                .NotEmpty()
                .MaximumLength(1_000)
                .When(request => request.Update.Dosage is not null);
            RuleFor(request => request.Update.Quantity)
                .GreaterThan(0)
                .When(request => request.Update.Quantity.HasValue);
            RuleFor(request => request.Update)
                .Must(update => update.MedicationId.HasValue ||
                    update.Dosage is not null ||
                    update.Quantity.HasValue)
                .WithMessage("Debe especificar al menos un campo para actualizar.");
        });
    }
}
