using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure;
using PsyLink.Infrastructure.Persistence;
using PsyLink.Infrastructure.Repositories.Clinical;
using Xunit;

namespace PsyLink.Tests;

public sealed class AppointmentNotesRepositoryTests
{
    [Fact]
    public void AddInfrastructure_ResolvesAppointmentNotesRepository()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=localhost;Port=5432;Database=psylink;Username=postgres;Password=postgres"
            })
            .Build();
        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var repository = scope.ServiceProvider
            .GetRequiredService<IAppointmentNotesRepository>();

        Assert.IsType<AppointmentNotesRepository>(repository);
    }

    [Fact]
    public async Task SaveAsync_PersistsNotesAndCompletedStatus_WithinTenant()
    {
        var tenantId = Guid.NewGuid();
        var appointment = Appointment.Schedule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            tenantId,
            new DateOnly(2026, 11, 20),
            new TimeOnly(10, 0),
            new TimeOnly(11, 0));
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(
            options,
            new TestCurrentUser(tenantId));
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        var repository = new AppointmentNotesRepository(context);
        var updatedAppointment = await context.Appointments
            .SingleAsync(item => item.AppointmentId == appointment.AppointmentId);
        updatedAppointment.Complete();

        var updatedAt = await repository.SaveAsync(
            updatedAppointment,
            tenantId,
            "Notas de sesión",
            CancellationToken.None);
        await context.SaveChangesAsync();

        var notes = await repository.GetByAppointmentIdAsync(
            appointment.AppointmentId,
            tenantId);
        var otherTenantNotes = await repository.GetByAppointmentIdAsync(
            appointment.AppointmentId,
            Guid.NewGuid());

        Assert.NotNull(notes);
        Assert.Equal("Notas de sesión", notes.Notes);
        Assert.Equal(updatedAt, notes.UpdatedAt);
        Assert.Null(otherTenantNotes);
        Assert.Equal(
            AppointmentStatus.Completed,
            (await context.Appointments.SingleAsync()).Status);
    }

    private sealed class TestCurrentUser(Guid tenantId) : ICurrentUserService
    {
        public bool IsAuthenticated => true;
        public Guid UserId => Guid.NewGuid();
        public Guid? TenantId { get; } = tenantId;
        public string? RoleName => "PSYCHOLOGIST";
    }
}
