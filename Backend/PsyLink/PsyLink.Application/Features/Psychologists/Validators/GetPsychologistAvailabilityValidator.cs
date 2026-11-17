using FluentValidation;
using PsyLink.Application.Features.Psychologists.Services.Queries;

namespace PsyLink.Application.Features.Psychologists.Validators;

public sealed class GetPsychologistAvailabilityValidator : AbstractValidator<GetPsychologistAvailabilityQuery>
{
    public GetPsychologistAvailabilityValidator()
    {
        RuleFor(query => query.PsychologistId).NotEmpty();
        RuleFor(query => query.Availability).NotNull();
        When(query => query.Availability is not null, () =>
        {
            RuleFor(query => query.Availability.DateFrom)
                .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("La fecha inicial no puede ser anterior a hoy.");
            RuleFor(query => query.Availability.DateTo)
                .Must((query, dateTo) =>
                    dateTo >= query.Availability.DateFrom &&
                    dateTo.DayNumber - query.Availability.DateFrom.DayNumber <= 31)
                .WithMessage("El rango debe ser válido y no puede superar 31 días.");
            RuleFor(query => query.Availability.DurationMinutes)
                .NotNull()
                .GreaterThan(0)
                .LessThanOrEqualTo(1440)
                .WithMessage("Debe especificar una duración válida en minutos.");
            RuleFor(query => query.Availability.Timezone)
                .NotEmpty()
                .Must(IsTimeZoneValid)
                .WithMessage("La zona horaria especificada no es válida.");
        });
    }

    private static bool IsTimeZoneValid(string timezone)
    {
        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
    }
}
