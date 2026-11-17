
using PsyLink.Application.Interfaces;

namespace PsyLink.Infrastructure.Security;

public sealed class PasswordVerifier : IPasswordVerifier
{
    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) ||
            string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        return PasswordHashing.Verify(password, passwordHash);
    }
}
