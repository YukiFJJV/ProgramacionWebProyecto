namespace PsyLink.Application.Features.Psychologists.DTOs;

public sealed record PsychologistQueryDto(
    string? Search,
    Guid? SpecialtyId,
    DateOnly? AvailableOn,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "firstName",
    string SortDirection = "asc");
