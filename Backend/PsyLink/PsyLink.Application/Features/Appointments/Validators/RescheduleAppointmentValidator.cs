using FluentValidation;
using PsyLink.Application.Features.Appointments.Services.Commands;

namespace PsyLink.Application.Features.Appointments.Validators;

public sealed class RescheduleAppointmentValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentValidator()
    {
        RuleFor(command => command.AppointmentId).NotEmpty();
        RuleFor(command => command.Update).NotNull();
        When(command => command.Update is not null, () =>
        {
            RuleFor(command => command.Update)
                .Must(update =>
                    update.PsychologistId.HasValue ||
                    update.DateAt.HasValue ||
                    update.StartsAt.HasValue ||
                    update.EndsAt.HasValue)
                .WithMessage("Debe proporcionar al menos un cambio para la cita.");
            RuleFor(command => command.Update)
                .Must(update =>
                    !update.StartsAt.HasValue ||
                    !update.EndsAt.HasValue ||
                    update.EndsAt > update.StartsAt)
                .WithMessage("La hora final debe ser posterior a la hora inicial.");
            RuleFor(command => command.Update.DateAt)
                .Must(dateAt =>
                    !dateAt.HasValue ||
                    dateAt.Value >= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("La fecha de la cita no puede ser anterior a hoy.");
        });
    }
}
