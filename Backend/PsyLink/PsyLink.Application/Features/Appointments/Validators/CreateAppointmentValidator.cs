using FluentValidation;
using PsyLink.Application.Features.Appointments.Services.Commands;

namespace PsyLink.Application.Features.Appointments.Validators;

public sealed class CreateAppointmentValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentValidator()
    {
        RuleFor(command => command.Appointment).NotNull();
        When(command => command.Appointment is not null, () =>
        {
            RuleFor(command => command.Appointment.PsychologistId).NotEmpty();
            RuleFor(command => command.Appointment.DateAt)
                .Must(dateAt => dateAt >= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("La fecha de la cita no puede ser anterior a hoy.");
            RuleFor(command => command.Appointment.EndsAt)
                .Must((command, endsAt) => endsAt > command.Appointment.StartsAt)
                .WithMessage("La hora final debe ser posterior a la hora inicial.");
        });
    }
}
