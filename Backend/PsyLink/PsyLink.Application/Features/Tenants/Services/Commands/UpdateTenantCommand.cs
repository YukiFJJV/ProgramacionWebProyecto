using MediatR;
using PsyLink.Application.Features.Tenants.DTOs;

namespace PsyLink.Application.Features.Tenants.Services.Commands;

public record UpdateTenantCommand(
    Guid TenantId,
    string? Name,
    string? Email,
    bool EmailSpecified,
    string? Rtn,
    bool RtnSpecified,
    string? Phone,
    bool PhoneSpecified,
    bool? Active) : ICommand<TenantResponseDto>;
