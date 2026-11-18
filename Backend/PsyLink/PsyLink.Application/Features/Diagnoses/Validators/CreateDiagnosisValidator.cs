using FluentValidation;
using PsyLink.Application.Features.Diagnoses.Services.Commands;

namespace PsyLink.Application.Features.Diagnoses.Validators;

public sealed class CreateDiagnosisValidator : AbstractValidator<CreateDiagnosisCommand>
{
    public CreateDiagnosisValidator()
    {
        RuleFor(command => command.Diagnosis).NotNull();
        When(command => command.Diagnosis is not null, () =>
        {
            RuleFor(command => command.Diagnosis.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(name => !string.IsNullOrWhiteSpace(name));
        });
    }
}
