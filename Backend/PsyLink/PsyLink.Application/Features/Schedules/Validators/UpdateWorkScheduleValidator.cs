using FluentValidation;
using PsyLink.Application.Features.Schedules.Services.Commands;
using PsyLink.Domain.Clinical;

namespace PsyLink.Application.Features.Schedules.Validators;

public sealed class UpdateWorkScheduleValidator : AbstractValidator<UpdateWorkScheduleCommand>
{
    public UpdateWorkScheduleValidator()
    {
        RuleFor(command => command.ScheduleId).NotEmpty();
        RuleFor(command => command.Schedule).NotNull();
        When(command => command.Schedule is not null, () =>
        {
            RuleFor(command => command.Schedule.WeekDay)
                .Must(day => Enum.IsDefined(typeof(WeekDay), day))
                .WithMessage("El día de la semana no es válido.");
            RuleFor(command => command.Schedule.EndsAt)
                .Must((command, endsAt) => endsAt > command.Schedule.StartsAt)
                .WithMessage("La hora final debe ser posterior a la hora inicial.");
        });
    }
}
