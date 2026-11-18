using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PsyLink.Application.Interfaces;

namespace PsyLink.Infrastructure.Persistence;

public sealed class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=psylink;Username=postgres;Password=postgres")
            .Options;

        return new ApplicationDbContext(options, new DesignTimeCurrentUser());
    }

    private sealed class DesignTimeCurrentUser : ICurrentUserService
    {
        public bool IsAuthenticated => false;
        public Guid UserId => Guid.Empty;
        public Guid? TenantId => null;
        public string? RoleName => null;
    }
}
