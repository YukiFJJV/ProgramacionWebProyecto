using SaludMental.Domain.Identity;

namespace SaludMental.Domain.Pharmacy;

public class Laboratory
{
    public Guid LaboratoryId { get; set; }
    public Guid? PhoneId { get; set; }
    public string Name { get; set; } = string.Empty;

    public PhoneNumber? Phone { get; set; }
}
