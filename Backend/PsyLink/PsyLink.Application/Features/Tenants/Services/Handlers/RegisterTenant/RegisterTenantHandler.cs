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
            throw new PsyLink.Application.Common.Exceptions.ConflictException(
                "Ya existe una empresa con ese nombre.");
        }

        if (!string.IsNullOrWhiteSpace(request.Rtn) &&
            await tenantRepository.ExistsByRtnAsync(request.Rtn, cancellationToken: cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.ConflictException(
                "Ya existe una empresa con ese RTN.");
        }

        var phone = string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? null
            : new PhoneNumber
            {
                PhoneId = Guid.NewGuid(),
                Number = request.PhoneNumber.Trim()
            };
        var tenant = new Tenant
        {
            TenantId = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Rtn = string.IsNullOrWhiteSpace(request.Rtn) ? null : request.Rtn.Trim(),
            PhoneId = phone?.PhoneId,
            Phone = phone,
            Active = true
        };

        await tenantRepository.AddAsync(tenant, cancellationToken);

        return new TenantDto
        {
            TenantId = tenant.TenantId,
            Name = tenant.Name,
            Email = tenant.Email,
            Phone = tenant.Phone?.Number,
            PhoneId = tenant.Phone?.PhoneId,
            Rtn = tenant.Rtn,
            Active = tenant.Active
        };
    }
}
