namespace PsyLink.Domain.Identity;

public class PhoneNumber
{
    public Guid PhoneId { get; set; }
    public Guid? UserId { get; set; }

    /// <summary>Ocho dígitos (CHAR(8)).</summary>
    public string Number { get; set; } = string.Empty;
    public User? User { get; set; }
}
