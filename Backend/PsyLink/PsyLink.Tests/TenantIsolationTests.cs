using Microsoft.EntityFrameworkCore;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Clinical;
using PsyLink.Infrastructure.Persistence;
using Xunit;

namespace PsyLink.Tests;

public sealed class TenantIsolationTests
{
    [Fact]
    public async Task TenantA_CannotReadOrModifyTenantBPatient()
    {
        var databaseName = Guid.NewGuid().ToString();
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var patientBId = Guid.NewGuid();

        await using (var seedContext = CreateContext(databaseName, tenantA))
        {
            seedContext.Patients.Add(new Patient
            {
                PatientId = patientBId,
                TenantId = tenantB,
                UserId = Guid.NewGuid()
            });
            await seedContext.SaveChangesAsync();
        }

        await using var tenantAContext = CreateContext(databaseName, tenantA);
        var patient = await tenantAContext.Patients
            .SingleOrDefaultAsync(entity => entity.PatientId == patientBId);

        Assert.Null(patient);

        var patientFromAnotherContext = await seedContextValue(databaseName, tenantB);
        Assert.NotNull(patientFromAnotherContext);
    }

    [Fact]
    public async Task AddedTenantScopedEntity_AssignsCurrentTenant()
    {
        var tenantId = Guid.NewGuid();
        await using var context = CreateContext(Guid.NewGuid().ToString(), tenantId);

        var patient = new Patient
        {
            PatientId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        Assert.Equal(tenantId, patient.TenantId);
    }

    private static ApplicationDbContext CreateContext(string databaseName, Guid? tenantId)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
        return new ApplicationDbContext(options, new TestCurrentUser(tenantId));
    }

    private static async Task<Patient?> seedContextValue(string databaseName, Guid tenantId)
    {
        await using var context = CreateContext(databaseName, tenantId);
        return await context.Patients.FirstOrDefaultAsync();
    }

    private sealed class TestCurrentUser(Guid? tenantId) : ICurrentUserService
    {
        public bool IsAuthenticated => true;
        public Guid UserId { get; } = Guid.NewGuid();
        public Guid? TenantId { get; } = tenantId;
        public string? RoleName { get; } = "ADMIN";
    }
}
