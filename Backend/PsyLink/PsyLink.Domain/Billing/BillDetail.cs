namespace PsyLink.Domain.Billing;

public class BillDetail
{
    public Guid DetailId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid TenantId { get; set; }
    public string Detail { get; set; } = string.Empty;
    public short CantHours { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }

    public PaymentHistory Payment { get; set; } = null!;
}
