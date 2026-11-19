using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.Tenants.DTOs;
using PsyLink.Application.Features.Shared.DTOs;
using PsyLink.Application.Features.Tenants.Services.Commands;
using PsyLink.Application.Features.Tenants.Services.Queries;
using PsyLink.API.Authorization;

namespace PsyLink.API.Controllers;

[ApiController]
[Route("tenants")]
[AllowAnonymous]
public sealed class TenantsController(ISender mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    [EndpointSummary("Register a tenant")]
    [EndpointDescription("Creates an active tenant using the supplied company contact details.")]
    [ProducesResponseType<TenantResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TenantResponseDto>> Create(
        [FromBody] TenantCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RegisterTenantCommand(
                request.Name,
                request.Email,
                request.PhoneNumber,
                request.Rtn),
            cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { tenantId = result.TenantId },
            result);
    }

    [HttpGet]
    [Authorize(Roles = Roles.SuperAdmin)]
    [EndpointSummary("List tenants")]
    [EndpointDescription("Returns a paged, searchable, filterable, and sortable tenant list.")]
    [ProducesResponseType<PageResponseDto<TenantResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PageResponseDto<TenantResponseDto>>> Get(
        [FromQuery] TenantQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetTenantsQuery(
                query.Search,
                query.Active,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.SortDirection),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{tenantId:guid}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    [EndpointSummary("Get a tenant by ID")]
    [EndpointDescription("Returns the tenant and its contact details for the supplied identifier.")]
    [ProducesResponseType<TenantResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantResponseDto>> GetById(
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetTenantsQuery(Search: tenantId.ToString(), PageSize: 1),
            cancellationToken);

        return result.Items.Count == 0
            ? NotFound()
            : Ok(result.Items[0]);
    }

    [HttpPut("{tenantId:guid}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    [EndpointSummary("Update a tenant")]
    [EndpointDescription("Updates supplied tenant details and activation state.")]
    [ProducesResponseType<TenantResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TenantResponseDto>> Update(
        Guid tenantId,
        [FromBody] TenantUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateTenantCommand(
                tenantId,
                request.Name,
                request.Email,
                request.EmailSpecified,
                request.Rtn,
                request.RtnSpecified,
                request.PhoneNumber,
                request.PhoneNumberSpecified,
                request.Active),
            cancellationToken);

        return Ok(result);
    }
}
