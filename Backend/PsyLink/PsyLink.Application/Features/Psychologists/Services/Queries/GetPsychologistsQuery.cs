using MediatR;
using PsyLink.Application.Features.Psychologists.DTOs;
using PsyLink.Application.Features.Shared.DTOs;

namespace PsyLink.Application.Features.Psychologists.Services.Queries;

public record GetPsychologistsQuery(
    string? Search = null,
    Guid? SpecialtyId = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PageResponseDto<PsychologistSummaryResponseDto>>;
