using PsyLink.Domain.Identity;

namespace PsyLink.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
