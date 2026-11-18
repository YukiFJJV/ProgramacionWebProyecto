using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using PsyLink.API;
using PsyLink.Application.Features.Appointments.DTOs;
using PsyLink.Application.Features.Authentication.DTOs;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Shared.DTOs;
using PsyLink.Application.Features.PatientRecords.DTOs;

namespace PsyLink.Tests;

public class DtoContractTests
{
    [Fact]
    public void ApiAssemblyDoesNotDefineDtoTypes()
    {
        var apiAssembly = typeof(DependencyInjection).Assembly;
        var violations = apiAssembly
            .GetTypes()
            .Where(type =>
                type.Name.EndsWith("Dto", StringComparison.Ordinal) ||
                type.Name.EndsWith("Dto`1", StringComparison.Ordinal))
            .Select(type => type.FullName)
            .ToArray();

        Assert.Empty(violations);
    }

    [Fact]
    public void ApplicationDtoNamesAreUnique()
    {
        var applicationAssembly = typeof(PageResponseDto<>).Assembly;
        var duplicates = applicationAssembly
            .GetTypes()
            .Where(type => type.Name.Contains("Dto", StringComparison.Ordinal))
            .GroupBy(type => type.Name.Replace("`1", string.Empty))
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void RequiredDocumentedDtoTypesExistInApplication()
    {
        var applicationAssembly = typeof(PageResponseDto<>).Assembly;
        string[] names =
        [
            "LoginRequestDto",
            "LoginResponseDto",
            "TenantCreateRequestDto",
            "TenantUpdateRequestDto",
            "TenantResponseDto",
            "UserCreateRequestDto",
            "UserResponseDto",
            "PatientCreateRequestDto",
            "PatientResponseDto",
            "AppointmentCreateRequestDto",
            "AppointmentResponseDto",
            "PageResponseDto`1",
            "RecordDetailResponseDto",
            "PrescriptionResponseDto"
        ];

        foreach (var name in names)
        {
            var found = applicationAssembly.GetTypes().Any(type =>
                type.Name.Equals(name, StringComparison.Ordinal));
            Assert.True(found, $"DTO documentado no encontrado: {name}");
        }
    }

    [Fact]
    public void SerializationUsesCamelCaseAndContractFormats()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(allowIntegerValues: false),
                new TimeOnlyJsonConverter()
            }
        };

        var login = new LoginResponseDto
        {
            AccessToken = "token",
            TokenType = "Bearer",
            ExpiresIn = 3600,
            User = new UserSessionDto
            {
                UserId = Guid.NewGuid(),
                TenantId = null,
                Email = "admin@example.com",
                FirstName = "Ana",
                LastName = "Perez"
            }
        };
        var loginJson = JsonSerializer.Serialize(login, options);
        Assert.Contains("\"accessToken\"", loginJson);
        Assert.Contains("\"expiresIn\"", loginJson);

        var appointment = new AppointmentResponseDto
        {
            AppointmentId = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            PsychologistId = Guid.NewGuid(),
            DateAt = new DateOnly(2026, 11, 20),
            StartsAt = new TimeOnly(10, 0),
            EndsAt = new TimeOnly(10, 50)
        };
        var appointmentJson = JsonSerializer.Serialize(appointment, options);
        Assert.Contains("\"2026-11-20\"", appointmentJson);
        Assert.Contains("\"10:00\"", appointmentJson);

        var page = new PageResponseDto<AppointmentResponseDto>(
            [appointment],
            1,
            20,
            1,
            1,
            false,
            false);
        var pageJson = JsonSerializer.Serialize(page, options);
        Assert.Contains("\"hasPrevious\":false", pageJson);
        Assert.Contains("\"hasNext\":false", pageJson);

        var patient = new PatientResponseDto
        {
            PatientId = Guid.NewGuid(),
            BirthDate = new DateOnly(1995, 6, 15),
            Age = 31
        };
        var patientJson = JsonSerializer.Serialize(patient, options);
        Assert.Contains("\"birthDate\"", patientJson);
    }
}
