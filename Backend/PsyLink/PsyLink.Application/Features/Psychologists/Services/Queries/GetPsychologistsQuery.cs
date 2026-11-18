using MediatR;
using PsyLink.Application.Features.Psychologists.DTOs;
using PsyLink.Application.Features.Shared.DTOs;

namespace PsyLink.Application.Features.Psychologists.Services.Queries;

public record GetPsychologistsQuery(
    string? Search = null,
    Guid? SpecialtyId = null,
    DateOnly? AvailableOn = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "lastName",
    string SortDirection = "asc") : IQuery<PageResponseDto<PsychologistSummaryResponseDto>>;
