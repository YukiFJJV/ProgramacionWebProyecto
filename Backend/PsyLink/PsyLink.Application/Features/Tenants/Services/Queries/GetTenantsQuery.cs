using MediatR;
using PsyLink.Application.Features.Tenants.DTOs;

namespace PsyLink.Application.Features.Tenants.Services.Queries;

public record GetTenantsQuery(
    string? Search = null,
    bool? Active = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "name",
    string SortDirection = "asc") : IQuery<TenantPageDto>;
