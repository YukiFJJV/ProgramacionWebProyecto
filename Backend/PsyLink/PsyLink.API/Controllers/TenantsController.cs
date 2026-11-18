using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.API.DTOs;
using PsyLink.Application.Features.Tenants.DTOs;
using PsyLink.Application.Features.Tenants.Services.Commands;
using PsyLink.Application.Features.Tenants.Services.Queries;
using PsyLink.API.Authorization;

namespace PsyLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Route("tenants")]
public sealed class TenantsController(IMediator mediator) : ControllerBase
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
        var response = ToResponse(result);

        return Created($"/tenants/{response.TenantId}", response);
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

        var totalPages = (int)Math.Ceiling(result.TotalCount / (double)result.PageSize);
        var response = new PageResponseDto<TenantResponseDto>(
            result.Items.Select(ToResponse).ToList(),
            result.PageNumber,
            result.PageSize,
            result.TotalCount,
            totalPages,
            result.PageNumber > 1,
            result.PageNumber < totalPages);

        return Ok(response);
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

        if (result.Items.Count == 0)
        {
            return NotFound();
        }

        return Ok(ToResponse(result.Items[0]));
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

        return Ok(ToResponse(result));
    }

    private static TenantResponseDto ToResponse(
        TenantDto tenant) =>
        new(
            tenant.TenantId,
            tenant.Name,
            tenant.Email,
            tenant.Rtn,
            tenant.Phone is null || tenant.PhoneId is null
                ? null
                : new PhoneNumberResponseDto(tenant.PhoneId.Value, tenant.Phone),
            tenant.Active);
}
