using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.Application.Features.Users.Services.Commands;
using PsyLink.Application.Features.Users.Services.Queries;
using PsyLink.API.Authorization;

using PsyLink.Application.Features.Users.DTOs;
using PsyLink.Application.Features.Shared.DTOs;
namespace PsyLink.API.Controllers;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("users")]
public sealed class UsersController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a user")]
    [EndpointDescription("Creates a user in the authenticated administrator's tenant with an assignable role.")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponseDto>> Create(
        [FromBody] UserCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateUserCommand(request), cancellationToken);
        return result == null ? BadRequest() : CreatedAtAction(
            nameof(GetById),
            new { userId = result.UserId },
            result);
    }

    [HttpGet]
    [EndpointSummary("List users in the authenticated tenant")]
    [EndpointDescription("Returns a paged user list with search, role filtering, and sorting.")]
    [ProducesResponseType<PageResponseDto<UserResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PageResponseDto<UserResponseDto>>> Get(
        [FromQuery] UserQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetUsersQuery(
                query.Search,
                query.RoleId,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.SortDirection),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{userId:guid}")]
    [EndpointSummary("Get a tenant user by ID")]
    [EndpointDescription("Returns a user belonging to the authenticated administrator's tenant.")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> GetById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{userId:guid}")]
    [EndpointSummary("Update a tenant user")]
    [EndpointDescription("Updates supplied user details, contact phone, role, or access state.")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponseDto>> Update(
        Guid userId,
        [FromBody] UserUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateUserCommand(
                userId,
                request.Email,
                request.EmailSpecified,
                request.FirstName,
                request.FirstNameSpecified,
                request.LastName,
                request.LastNameSpecified,
                request.PhoneNumber,
                request.PhoneNumberSpecified,
                request.RoleId,
                request.RoleIdSpecified,
                request.Active,
                request.ActiveSpecified),
            cancellationToken);

        return Ok(result);
    }
}
