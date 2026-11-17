
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PsyLink.Application.Interfaces;
using PsyLink.Infrastructure.Persistence;

using PsyLink.Infrastructure.Repositories.Identity;
using PsyLink.Infrastructure.Repositories.Clinical;
using PsyLink.Infrastructure.Repositories.Pharmacy;
using PsyLink.Infrastructure.Security;
using PsyLink.Infrastructure.Services;

namespace PsyLink.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // CONNECTION STRING
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No se encontró la cadena de conexión DefaultConnection.");
        }

        // DATABASE
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // IDENTITY REPOSITORIES
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // CLINICAL REPOSITORIES
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IPsychologistRepository, PsychologistRepository>();
        services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
        services.AddScoped<IDiagnosisCatalogRepository, DiagnosisCatalogRepository>();
        services.AddScoped<IMedicationRepository, MedicationRepository>();
        services.AddScoped<IPatientRecordRepository, PatientRecordRepository>();
        services.AddScoped<IRecordDiagnosisRepository, RecordDiagnosisRepository>();
        services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();

        // PHARMACY REPOSITORIES
        services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
        // SECURITY SERVICES
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IPasswordVerifier, PasswordVerifier>();
       
        services.Configure<JwtSettings>(
        configuration.GetSection("Jwt"));

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        // HTTP CONTEXT
        services.AddHttpContextAccessor();
        // CURRENT USER SERVICE
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
