namespace SaludMental.Domain.Identity;

/// <summary>Empresa cliente de la plataforma (multi-tenant).</summary>
public class Tenant
{
    public Guid TenantId { get; set; }
    public Guid? PhoneId { get; set; }

    /// <summary>Nombre comercial único (VARCHAR(100)).</summary>
    public string Name { get; set; } = string.Empty;

    public bool Active { get; set; }
    public string? Email { get; set; }
    public string? Rtn { get; set; }

    /// <summary>Token para afiliar usuarios a la empresa.</summary>
    public string? LinkToken { get; set; }

    public PhoneNumber? Phone { get; set; }
}
