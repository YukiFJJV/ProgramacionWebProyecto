namespace PsyLink.Domain.Identity;

public class User
{
    public Guid UserId { get; set; }

    /// <summary>Null únicamente para cuentas globales SUPER_ADMIN.</summary>
    public Guid? TenantId { get; set; }

    public Guid? PhoneId { get; set; }
    public Guid RoleId { get; set; }

    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    /// <summary>Columna nueva (no está en el ER original): HU-22 valida que el usuario esté activo.</summary>
    public bool Active { get; set; } = true;

    public Tenant? Tenant { get; set; }
    public PhoneNumber? Phone { get; set; }
    public Role Role { get; set; } = null!;
}
