using MediatR;
using PsyLink.Application.Features.Psychologists.DTOs;
using PsyLink.Application.Features.Psychologists.Services.Queries;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Psychologists.Services.Handlers.GetPsychologistAvailability;

public sealed class GetPsychologistAvailabilityHandler(
    IPsychologistRepository psychologistRepository,
    IWorkScheduleRepository workScheduleRepository,
    IAppointmentRepository appointmentRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<GetPsychologistAvailabilityQuery, IReadOnlyList<AvailabilityResponseDto>>
{
    public async Task<IReadOnlyList<AvailabilityResponseDto>> Handle(
        GetPsychologistAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = currentUser.TenantId;
        if (tenantId is null ||
            (currentUser.RoleName != RoleNames.Admin &&
             currentUser.RoleName != RoleNames.Patient &&
             currentUser.RoleName != RoleNames.Psychologist))
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException(
                "La consulta de disponibilidad requiere un administrador, paciente o psicólogo autenticado.");
        }

        var psychologist = await psychologistRepository.GetByIdForTenantAsync(
                request.PsychologistId,
                tenantId.Value,
                cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el psicólogo {request.PsychologistId}.");
        if (!psychologist.User.Active)
        {
            throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró el psicólogo {request.PsychologistId}.");
        }

        var schedules = await workScheduleRepository.GetByPsychologistAsync(
            psychologist.PsychologistId,
            tenantId.Value,
            cancellationToken);
        var appointments = await appointmentRepository.GetByPsychologistAndDateRangeAsync(
            psychologist.PsychologistId,
            request.Availability.DateFrom,
            request.Availability.DateTo,
            cancellationToken);
        var timezone = TimeZoneInfo.FindSystemTimeZoneById(request.Availability.Timezone);
        var duration = TimeSpan.FromMinutes(request.Availability.DurationMinutes
            ?? throw new PsyLink.Application.Common.Exceptions.ConflictException("La duración de los slots debe especificarse."));
        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timezone);
        var result = new List<AvailabilityResponseDto>();

        for (var date = request.Availability.DateFrom;
             date <= request.Availability.DateTo;
             date = date.AddDays(1))
        {
            var day = date.ToWeekDay();
            var response = new AvailabilityResponseDto
            {
                PsychologistId = psychologist.PsychologistId,
                Date = date
            };

            foreach (var schedule in schedules.Where(schedule => schedule.WeekDay == day))
            {
                var startsAt = schedule.StartsAt;
                while (startsAt.ToTimeSpan() + duration <= schedule.EndsAt.ToTimeSpan())
                {
                    var endsAt = TimeOnly.FromTimeSpan(startsAt.ToTimeSpan() + duration);
                    if (IsFuture(date, startsAt, now, timezone) &&
                        IsValidLocalTime(date, startsAt, timezone) &&
                        IsValidLocalTime(date, endsAt, timezone) &&
                        !appointments.Any(appointment =>
                            appointment.Status is AppointmentStatus.Pending or AppointmentStatus.Confirmed &&
                            appointment.DateAt == date &&
                            startsAt < appointment.EndsAt &&
                            appointment.StartsAt < endsAt))
                    {
                        response.Slots.Add(new AvailabilitySlotDto
                        {
                            StartsAt = ToDateTimeOffset(date, startsAt, timezone),
                            EndsAt = ToDateTimeOffset(date, endsAt, timezone)
                        });
                    }

                    startsAt = TimeOnly.FromTimeSpan(startsAt.ToTimeSpan() + duration);
                }
            }

            result.Add(response);
        }

        return result;
    }

    private static bool IsFuture(
        DateOnly date,
        TimeOnly startsAt,
        DateTimeOffset now,
        TimeZoneInfo timezone)
    {
        var localNow = TimeZoneInfo.ConvertTime(now, timezone);
        var localDate = DateOnly.FromDateTime(localNow.DateTime);
        return date > localDate || (date == localDate && startsAt > TimeOnly.FromDateTime(localNow.DateTime));
    }

    private static DateTimeOffset ToDateTimeOffset(DateOnly date, TimeOnly time, TimeZoneInfo timezone)
    {
        var local = DateTime.SpecifyKind(date.ToDateTime(time), DateTimeKind.Unspecified);
        return new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(local, timezone), TimeSpan.Zero);
    }

    private static bool IsValidLocalTime(DateOnly date, TimeOnly time, TimeZoneInfo timezone)
    {
        var local = DateTime.SpecifyKind(date.ToDateTime(time), DateTimeKind.Unspecified);
        return !timezone.IsInvalidTime(local);
    }
}
