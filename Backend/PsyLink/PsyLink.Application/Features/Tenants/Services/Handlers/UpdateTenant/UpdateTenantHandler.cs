using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PsyLink.Application.Features.Tenants.DTOs;
using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.Tenants.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Application.Features.Tenants.Services.Handlers.UpdateTenant;

public sealed class UpdateTenantHandler(
    ITenantRepository tenantRepository,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateTenantCommand, TenantResponseDto>
{
    public async Task<TenantResponseDto> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.RoleName != RoleNames.SuperAdmin)
        {
            throw new PsyLink.Application.Common.Exceptions.ForbiddenException("La operación requiere el rol SUPER_ADMIN.");
        }

        var tenant = await tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new PsyLink.Application.Common.Exceptions.NotFoundException($"No se encontró la empresa {request.TenantId}.");

        if (request.Name is not null &&
            await tenantRepository.ExistsByNameAsync(request.Name, request.TenantId, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.ConflictException(
                "Ya existe una empresa con ese nombre.");
        }

        if (request.RtnSpecified &&
            request.Rtn is not null &&
            await tenantRepository.ExistsByRtnAsync(request.Rtn, request.TenantId, cancellationToken))
        {
            throw new PsyLink.Application.Common.Exceptions.ConflictException(
                "Ya existe una empresa con ese RTN.");
        }

        if (request.Name is not null)
        {
            tenant.Name = request.Name.Trim();
        }

        if (request.EmailSpecified)
        {
            tenant.Email = string.IsNullOrWhiteSpace(request.Email)
                ? null
                : request.Email.Trim();
        }

        if (request.RtnSpecified)
        {
            tenant.Rtn = string.IsNullOrWhiteSpace(request.Rtn)
                ? null
                : request.Rtn.Trim();
        }

        if (request.PhoneSpecified)
        {
            if (string.IsNullOrWhiteSpace(request.Phone))
            {
                tenant.Phone = null;
                tenant.PhoneId = null;
            }
            else
            {
                tenant.Phone ??= new PhoneNumber { PhoneId = Guid.NewGuid() };
                tenant.Phone.Number = request.Phone.Trim();
                tenant.PhoneId = tenant.Phone.PhoneId;
            }
        }

        if (request.Active.HasValue)
        {
            tenant.Active = request.Active.Value;
        }

        await tenantRepository.UpdateAsync(tenant, cancellationToken);

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
