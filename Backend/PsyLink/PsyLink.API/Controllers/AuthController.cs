using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsyLink.API.DTOs;
using PsyLink.Application.Features.Authentication.DTOs;
using PsyLink.Application.Features.Authentication.Services.Commands;

namespace PsyLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Route("auth")]
public sealed class AuthController(
    IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [EndpointSummary("Authenticate a user")]
    [EndpointDescription("Validates credentials and returns a bearer token with the user's session identity.")]
    [ProducesResponseType<PsyLink.Application.Features.Authentication.DTOs.LoginResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PsyLink.Application.Features.Authentication.DTOs.LoginResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        return Ok(result);
    }
}
