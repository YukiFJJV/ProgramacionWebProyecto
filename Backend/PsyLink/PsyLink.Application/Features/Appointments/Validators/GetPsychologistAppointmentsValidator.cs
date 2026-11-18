using FluentValidation;
using PsyLink.Application.Features.Appointments.Services.Queries;

namespace PsyLink.Application.Features.Appointments.Validators;

public sealed class GetPsychologistAppointmentsValidator : AbstractValidator<GetPsychologistAppointmentsQuery>
{
    public GetPsychologistAppointmentsValidator()
    {
        RuleFor(request => request.Filter).NotNull();
        When(request => request.Filter is not null, () =>
        {
            RuleFor(request => request.Filter.Page).GreaterThan(0);
            RuleFor(request => request.Filter.PageSize).InclusiveBetween(1, 100);
            RuleFor(request => request.Filter.Search).MaximumLength(200);
            RuleFor(request => request.Filter.PatientId)
                .Must(patientId => patientId is null || patientId != Guid.Empty)
                .WithMessage("El identificador del paciente no puede estar vacío.");
            RuleFor(request => request.Filter.PsychologistId)
                .Must(psychologistId => psychologistId is null || psychologistId != Guid.Empty)
                .WithMessage("El identificador del psicólogo no puede estar vacío.");
            RuleFor(request => request.Filter.DateTo)
                .GreaterThanOrEqualTo(request => request.Filter.DateFrom)
                .When(request => request.Filter.DateFrom.HasValue && request.Filter.DateTo.HasValue);
            RuleFor(request => request.Filter.SortDirection)
                .Must(direction => direction is not null &&
                    (direction.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                     direction.Equals("desc", StringComparison.OrdinalIgnoreCase)))
                .WithMessage("La dirección de ordenamiento debe ser asc o desc.");
        });
    }
}
