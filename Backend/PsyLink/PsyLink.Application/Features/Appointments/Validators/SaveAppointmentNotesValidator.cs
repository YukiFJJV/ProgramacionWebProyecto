using FluentValidation;
using PsyLink.Application.Features.Appointments.Services.Commands;

namespace PsyLink.Application.Features.Appointments.Validators;

public sealed class SaveAppointmentNotesValidator : AbstractValidator<SaveAppointmentNotesCommand>
{
    public SaveAppointmentNotesValidator()
    {
        RuleFor(request => request.AppointmentId).NotEmpty();
        RuleFor(request => request.Notes).NotNull();
        When(request => request.Notes is not null, () =>
        {
            RuleFor(request => request.Notes.Notes)
                .NotEmpty()
                .MaximumLength(10_000);
        });
    }
}
