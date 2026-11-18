using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PsyLink.Application.Features.Tenants.DTOs;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.Tenants.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Tenants.Services.Handlers.RegisterTenant;

public sealed class RegisterTenantHandler(ITenantRepository tenantRepository)
    : IRequestHandler<RegisterTenantCommand, TenantResponseDto>
{
    public async Task<TenantResponseDto> Handle(
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

        return new TenantResponseDto(
            tenant.TenantId,
            tenant.Name,
            tenant.Email,
            tenant.Rtn,
            tenant.Phone is null
                ? null
                : new PhoneNumberResponseDto
                {
                    PhoneId = tenant.Phone.PhoneId,
                    PhoneNumber = tenant.Phone.Number
                },
            tenant.Active);
    }
}
