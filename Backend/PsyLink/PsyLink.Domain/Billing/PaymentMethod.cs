namespace PsyLink.Domain.Billing;

public class PaymentMethod
{
    public Guid MethodId { get; set; }
    public string Method { get; set; } = string.Empty;
}
