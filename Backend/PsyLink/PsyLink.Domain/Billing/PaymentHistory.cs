namespace PsyLink.Domain.Billing;

public class PaymentHistory
{
    public Guid PaymentId { get; set; }
    public Guid MethodId { get; set; }
    public Guid FiscalAuthorizationId { get; set; }

    /// <summary>VARCHAR(200).</summary>
    public string? Description { get; set; }

    public DateTime IssueDate { get; set; }

    /// <summary>Número de factura (CHAR(16)).</summary>
    public string? BillNumber { get; set; }

    public decimal Discount { get; set; }
    public decimal Total { get; set; }

    public PaymentMethod Method { get; set; } = null!;
    public FiscalAuthorization FiscalAuthorization { get; set; } = null!;
    public ICollection<BillDetail> Details { get; set; } = new List<BillDetail>();
}
