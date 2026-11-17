
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using PsyLink.Application.Interfaces;
using PsyLink.Domain.Identity;

namespace PsyLink.Infrastructure.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public string GenerateToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (!user.Active)
        {
            throw new InvalidOperationException(
                "No se puede generar un token para un usuario inactivo.");
        }

        if (string.IsNullOrWhiteSpace(_settings.SecretKey) ||
            Encoding.UTF8.GetByteCount(_settings.SecretKey) < 32)
        {
            throw new InvalidOperationException(
                "La clave JWT debe tener al menos 32 bytes.");
        }

        if (string.IsNullOrWhiteSpace(_settings.Issuer) ||
            string.IsNullOrWhiteSpace(_settings.Audience))
        {
            throw new InvalidOperationException(
                "La configuración Issuer y Audience es obligatoria.");
        }

        if (_settings.ExpirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "La duración del token debe ser mayor que cero.");
        }

        if (user.Role is null ||
            string.IsNullOrWhiteSpace(user.Role.Name))
        {
            throw new InvalidOperationException(
                "El usuario no tiene un rol válido.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.TenantId.HasValue)
        {
            claims.Add(new Claim(
                "tenant_id",
                user.TenantId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _settings.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
