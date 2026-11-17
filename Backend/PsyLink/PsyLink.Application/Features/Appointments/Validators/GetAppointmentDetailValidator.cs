using FluentValidation;
using PsyLink.Application.Features.Appointments.Services.Queries;

namespace PsyLink.Application.Features.Appointments.Validators;

public sealed class GetAppointmentDetailValidator : AbstractValidator<GetAppointmentDetailQuery>
{
    public GetAppointmentDetailValidator()
    {
        RuleFor(request => request.AppointmentId).NotEmpty();
    }
}
