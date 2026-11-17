using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PsyLink.Application.Features.Tenants.DTOs;
using PsyLink.Application.Features.Tenants.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Tenants.Services.Handlers.UpdateTenant;

public sealed class UpdateTenantHandler(
    ITenantRepository tenantRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateTenantCommand, TenantDto>
{
    public async Task<TenantDto> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.RoleName != RoleNames.SuperAdmin)
        {
            throw new UnauthorizedAccessException("La operación requiere el rol SUPER_ADMIN.");
        }

        var tenant = await tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró la empresa {request.TenantId}.");

        if (await tenantRepository.ExistsByNameAsync(request.Name, request.TenantId, cancellationToken))
        {
            throw new ValidationException(
                new[] { new ValidationFailure(nameof(request.Name), "Ya existe una empresa con ese nombre.") });
        }

        if (await tenantRepository.ExistsByRtnAsync(request.Rtn, request.TenantId, cancellationToken))
        {
            throw new ValidationException(
                new[] { new ValidationFailure(nameof(request.Rtn), "Ya existe una empresa con ese RTN.") });
        }

        tenant.Phone ??= new PhoneNumber { PhoneId = Guid.NewGuid() };
        tenant.Phone.Number = request.Phone;
        tenant.PhoneId = tenant.Phone.PhoneId;
        tenant.Name = request.Name;
        tenant.Email = request.Email;
        tenant.Rtn = request.Rtn;
        tenant.Active = request.Active;

        await tenantRepository.UpdateAsync(tenant, cancellationToken);

        return new TenantDto
        {
            TenantId = tenant.TenantId,
            Name = tenant.Name,
            Email = tenant.Email,
            Phone = tenant.Phone.Number,
            Rtn = tenant.Rtn,
            Active = tenant.Active
        };
    }
}
