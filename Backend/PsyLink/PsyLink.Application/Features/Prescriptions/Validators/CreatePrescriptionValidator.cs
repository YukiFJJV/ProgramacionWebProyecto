using FluentValidation;
using PsyLink.Application.Features.Prescriptions.Services.Commands;

namespace PsyLink.Application.Features.Prescriptions.Validators;

public sealed class CreatePrescriptionValidator : AbstractValidator<CreatePrescriptionCommand>
{
    public CreatePrescriptionValidator()
    {
        RuleFor(request => request.RecordId).NotEmpty();
        RuleFor(request => request.Prescription).NotNull();
        When(request => request.Prescription is not null, () =>
        {
            RuleFor(request => request.Prescription.MedicationId).NotEmpty();
            RuleFor(request => request.Prescription.Dosage).NotEmpty().MaximumLength(1_000);
            RuleFor(request => request.Prescription.Quantity).GreaterThan(0);
        });
    }
}
