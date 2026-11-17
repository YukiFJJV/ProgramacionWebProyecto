using MediatR;
using PsyLink.Application.Features.Authentication.DTOs;

namespace PsyLink.Application.Features.Authentication.Services.Commands;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;
