using FluentValidation;
using MediatR;
using PsyLink.Application.Features.Authentication.DTOs;
using PsyLink.Application.Features.Authentication.Services.Commands;
using PsyLink.Application.Interfaces;
using PsyLink.Application.Features.Users.DTOs;

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
            throw new PsyLink.Application.Common.Exceptions.UnauthenticatedException("Correo electrónico o contraseña inválidos.");
        }

        if (string.IsNullOrWhiteSpace(user.Role?.Name))
        {
            throw new PsyLink.Application.Common.Exceptions.ConflictException("El usuario no tiene un rol válido asignado.");
        }

        var accessToken = tokenGenerator.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = accessToken,
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = tokenGenerator.ExpirationSeconds,
            UserId = user.UserId,
            Role = user.Role.Name,
            TenantId = user.TenantId,
            User = new UserSessionDto
            {
                UserId = user.UserId,
                TenantId = user.TenantId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = new RoleResponseDto
                {
                    RoleId = user.Role.RoleId,
                    Name = user.Role.Name
                }
            }
        };
    }
}
