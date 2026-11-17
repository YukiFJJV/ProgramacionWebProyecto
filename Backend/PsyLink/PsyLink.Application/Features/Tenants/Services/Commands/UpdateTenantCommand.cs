using MediatR;
using PsyLink.Application.Features.Tenants.DTOs;

namespace PsyLink.Application.Features.Tenants.Services.Commands;

public record UpdateTenantCommand(
    Guid TenantId,
    string Name,
    string Email,
    string Rtn,
    string Phone,
    bool Active) : IRequest<TenantDto>;
