using PsyLink.Domain.Common;
using PsyLink.Domain.Clinical;
using PsyLink.Domain.Pharmacy;
using Xunit;

namespace PsyLink.Tests;

public sealed class DomainRuleTests
{
    [Fact]
    public void Appointment_Schedule_RejectsInvalidTimeRange()
    {
        Assert.Throws<DomainException>(() =>
            Appointment.Schedule(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateOnly(2026, 11, 20),
                new TimeOnly(11, 0),
                new TimeOnly(10, 0)));
    }

    [Fact]
    public void Appointment_Reschedule_EnforcesOnePatientReschedule()
    {
        var appointment = Appointment.Schedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 11, 20),
            new TimeOnly(10, 0),
            new TimeOnly(11, 0));

        appointment.Reschedule(
            new DateOnly(2026, 11, 21),
            new TimeOnly(10, 0),
            new TimeOnly(11, 0));

        Assert.False(appointment.CanBeRescheduledByPatient);
    }

    [Fact]
    public void WorkSchedule_CreateAndUpdateValidateTimeRange()
    {
        var schedule = WorkSchedule.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            WeekDay.Monday,
            new TimeOnly(9, 0),
            new TimeOnly(12, 0));

        Assert.Throws<DomainException>(() =>
            schedule.Update(
                WeekDay.Monday,
                new TimeOnly(12, 0),
                new TimeOnly(11, 0)));
    }

    [Fact]
    public void Prescription_IssueAndUpdateValidateQuantity()
    {
        var prescription = Prescription.Issue(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "1 tableta",
            1);

        Assert.Throws<DomainException>(() =>
            prescription.Update("1 tableta", 0));
    }
}
