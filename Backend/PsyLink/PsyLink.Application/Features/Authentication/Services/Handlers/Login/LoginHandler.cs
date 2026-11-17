using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Authentication.DTOs;
using PsyLink.Application.Features.Authentication.Services.Commands;
using PsyLink.Application.Interfaces;

namespace PsyLink.Application.Features.Authentication.Services.Handlers.Login;

public sealed class LoginHandler(
    IUserRepository userRepository,
    IPasswordVerifier passwordVerifier,
    IJwtTokenGenerator tokenGenerator)
    : IRequestHandler<LoginCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !user.Active || !passwordVerifier.Verify(request.Password, user.PasswordHash))
        {
            throw new ValidationException("Correo electrónico o contraseña inválidos.");
        }

        if (string.IsNullOrWhiteSpace(user.Role?.Name))
        {
            throw new InvalidOperationException("El usuario no tiene un rol válido asignado.");
        }

        return new LoginResponseDto
        {
            Token = tokenGenerator.GenerateToken(user),
            UserId = user.UserId,
            Role = user.Role.Name,
            TenantId = user.TenantId
        };
    }
}
