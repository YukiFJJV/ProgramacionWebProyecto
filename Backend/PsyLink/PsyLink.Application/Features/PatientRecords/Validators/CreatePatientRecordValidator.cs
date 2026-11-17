using FluentValidation;
using PsyLink.Application.Features.PatientRecords.Services.Commands;

namespace PsyLink.Application.Features.PatientRecords.Validators;

public sealed class CreatePatientRecordValidator : AbstractValidator<CreatePatientRecordCommand>
{
    public CreatePatientRecordValidator()
    {
        RuleFor(request => request.Record).NotNull();
        When(request => request.Record is not null, () =>
        {
            RuleFor(request => request.Record.PatientId).NotEmpty();
            RuleFor(request => request.Record.RiskScore).InclusiveBetween(0, 3);
            RuleFor(request => request.Record.AppointmentId)
                .Must(appointmentId => appointmentId is null || appointmentId != Guid.Empty)
                .WithMessage("El identificador de la cita no puede estar vacío.");
        });
    }
}
