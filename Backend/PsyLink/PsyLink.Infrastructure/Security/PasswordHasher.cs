
using PsyLink.Application.Interfaces;

namespace PsyLink.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException(
                "La contraseña no puede estar vacía.",
                nameof(password));
        }

        return PasswordHashing.Hash(password);
    }
}
