using SaludMental.Domain.Clinical;
using SaludMental.Domain.Identity;

namespace SaludMental.Domain.Billing;

/// <summary>Autorización fiscal (CAI) para la emisión de facturas.</summary>
public class FiscalAuthorization
{
    public Guid FiscalAuthorizationId { get; set; }
    public Guid PatientId { get; set; }
    public Guid TenantId { get; set; }

    /// <summary>CAI (CHAR(32)).</summary>
    public string Cai { get; set; } = string.Empty;

    public DateOnly LimitDate { get; set; }

    /// <summary>Rango inicial de facturación (CHAR(16)).</summary>
    public string InitialRange { get; set; } = string.Empty;

    /// <summary>Rango final de facturación (CHAR(16)).</summary>
    public string FinalRange { get; set; } = string.Empty;

    public Patient Patient { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}
