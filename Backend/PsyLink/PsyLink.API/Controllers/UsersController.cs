using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.API.DTOs;
using PsyLink.Application.Features.Users.Services.Commands;
using PsyLink.Application.Features.Users.Services.Queries;
using PsyLink.API.Authorization;
using AppUserCreateRequestDto = PsyLink.Application.Features.Users.DTOs.UserCreateRequestDto;
using AppUserResponseDto = PsyLink.Application.Features.Users.DTOs.UserResponseDto;

namespace PsyLink.API.Controllers;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("api/[controller]")]
[Route("users")]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a tenant user")]
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
        var result = await mediator.Send(
            new CreateUserCommand(new AppUserCreateRequestDto
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                RoleId = request.RoleId
            }),
            cancellationToken);

        var response = ToResponse(result);
        return Created($"/users/{response.UserId}", response);
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

        var totalPages = (int)Math.Ceiling(result.TotalCount / (double)result.PageSize);
        var response = new PageResponseDto<UserResponseDto>(
            result.Items.Select(ToResponse).ToList(),
            result.PageNumber,
            result.PageSize,
            result.TotalCount,
            totalPages,
            result.PageNumber > 1,
            result.PageNumber < totalPages);

        return Ok(response);
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
        return Ok(ToResponse(result));
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

        return Ok(ToResponse(result));
    }

    private static UserResponseDto ToResponse(AppUserResponseDto user) =>
        new(
            user.UserId,
            user.TenantId,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Phone is null
                ? null
                : new PhoneNumberResponseDto(user.Phone.PhoneId, user.Phone.Number),
            new RoleResponseDto(user.Role.RoleId, user.Role.Name));
}
