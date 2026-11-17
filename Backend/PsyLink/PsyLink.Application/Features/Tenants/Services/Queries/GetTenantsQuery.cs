using MediatR;
using PsyLink.Application.Features.Tenants.DTOs;

namespace PsyLink.Application.Features.Tenants.Services.Queries;

public record GetTenantsQuery(
    Guid? TenantId = null,
    string? Name = null,
    string? Rtn = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<TenantPageDto>;
