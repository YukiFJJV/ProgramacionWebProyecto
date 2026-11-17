using FluentValidation;
using PsyLink.Application.Features.Prescriptions.Services.Commands;

namespace PsyLink.Application.Features.Prescriptions.Validators;

public sealed class DeletePrescriptionValidator : AbstractValidator<DeletePrescriptionCommand>
{
    public DeletePrescriptionValidator()
    {
        RuleFor(request => request.RecordId).NotEmpty();
        RuleFor(request => request.PrescriptionId).NotEmpty();
    }
}
