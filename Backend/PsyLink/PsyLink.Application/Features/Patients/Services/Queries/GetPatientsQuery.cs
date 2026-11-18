using MediatR;
using PsyLink.Application.Features.Patients.DTOs;
using PsyLink.Application.Features.Shared.DTOs;

namespace PsyLink.Application.Features.Patients.Services.Queries;

public record GetPatientsQuery(
    string? Search = null,
    Guid? PsychologistId = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "firstName",
    string SortDirection = "asc") : IQuery<PageResponseDto<PatientResponseDto>>;
