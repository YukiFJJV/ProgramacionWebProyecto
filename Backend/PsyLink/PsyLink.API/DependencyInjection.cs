using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PsyLink.API.Authorization;
using PsyLink.API.Middlewares;

namespace PsyLink.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddProblemDetails();
        services.AddExceptionHandler<ApiExceptionHandler>();

        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(allowIntegerValues: false)));

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        var jwt = configuration.GetSection("Jwt");
        var secretKey = jwt["SecretKey"];
        var issuer = jwt["Issuer"];
        var audience = jwt["Audience"];
        var expirationMinutes = jwt.GetValue<int?>("ExpirationMinutes") ?? 60;

        if (string.IsNullOrWhiteSpace(secretKey) ||
            Encoding.UTF8.GetByteCount(secretKey) < 32 ||
            string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience) ||
            expirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "La configuración Jwt debe incluir una clave de al menos 32 bytes, Issuer, Audience y una duración positiva.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.SuperAdminOnly,
                policy => policy.RequireRole(Roles.SuperAdmin));
            options.AddPolicy(Policies.AdminOnly,
                policy => policy.RequireRole(Roles.Admin));
            options.AddPolicy(Policies.PsychologistOnly,
                policy => policy.RequireRole(Roles.Psychologist));
            options.AddPolicy(Policies.PatientOnly,
                policy => policy.RequireRole(Roles.Patient));
            options.AddPolicy(Policies.AdminOrPsychologist,
                policy => policy.RequireRole(Roles.Admin, Roles.Psychologist));
            options.AddPolicy(Policies.AdminOrPatient,
                policy => policy.RequireRole(Roles.Admin, Roles.Patient));
            options.AddPolicy(Policies.AdminPsychologistOrPatient,
                policy => policy.RequireRole(Roles.Admin, Roles.Psychologist, Roles.Patient));
        });

        return services;
    }
}
