using MediatR;
using PsyLink.Application.Features.Tenants.DTOs;

namespace PsyLink.Application.Features.Tenants.Services.Commands;

public record RegisterTenantCommand(
    string Name,
    string? Email,
    string? PhoneNumber,
    string? Rtn) : ICommand<TenantDto>;
