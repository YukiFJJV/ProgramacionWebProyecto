namespace PsyLink.Application.Features.Patients.DTOs;

public sealed record PatientQueryDto(
    string? Search,
    Guid? PsychologistId,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "firstName",
    string SortDirection = "asc");
