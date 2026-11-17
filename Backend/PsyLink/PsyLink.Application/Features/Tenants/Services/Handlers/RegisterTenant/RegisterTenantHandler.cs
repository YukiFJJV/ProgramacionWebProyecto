using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PsyLink.Application.Features.Tenants.DTOs;
using PsyLink.Application.Features.Tenants.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Tenants.Services.Handlers.RegisterTenant;

public sealed class RegisterTenantHandler(ITenantRepository tenantRepository)
    : IRequestHandler<RegisterTenantCommand, TenantDto>
{
    public async Task<TenantDto> Handle(
        RegisterTenantCommand request,
        CancellationToken cancellationToken)
    {
        if (await tenantRepository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken))
        {
            throw new ValidationException(
                new[] { new ValidationFailure(nameof(request.Name), "Ya existe una empresa con ese nombre.") });
        }

        if (await tenantRepository.ExistsByRtnAsync(request.Rtn, cancellationToken: cancellationToken))
        {
            throw new ValidationException(
                new[] { new ValidationFailure(nameof(request.Rtn), "Ya existe una empresa con ese RTN.") });
        }

        var phone = new PhoneNumber
        {
            PhoneId = Guid.NewGuid(),
            Number = request.Phone
        };
        var tenant = new Tenant
        {
            TenantId = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Rtn = request.Rtn,
            PhoneId = phone.PhoneId,
            Phone = phone,
            Active = true
        };

        await tenantRepository.AddAsync(tenant, cancellationToken);

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
