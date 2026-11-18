using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PsyLink.API.DTOs;

/// <summary>Optional tenant fields to update; null clears nullable contact fields.</summary>
public sealed record TenantUpdateRequestDto : IValidatableObject
{
    private string? _name;
    private string? _email;
    private string? _rtn;
    private string? _phoneNumber;
    private bool? _active;

    public string? Name
    {
        get => _name;
        init
        {
            _name = value;
            NameSpecified = true;
        }
    }

    public string? Email
    {
        get => _email;
        init
        {
            _email = value;
            EmailSpecified = true;
        }
    }

    public string? Rtn
    {
        get => _rtn;
        init
        {
            _rtn = value;
            RtnSpecified = true;
        }
    }

    public string? PhoneNumber
    {
        get => _phoneNumber;
        init
        {
            _phoneNumber = value;
            PhoneNumberSpecified = true;
        }
    }

    public bool? Active
    {
        get => _active;
        init
        {
            _active = value;
            ActiveSpecified = true;
        }
    }

    [JsonIgnore]
    public bool NameSpecified { get; private init; }

    [JsonIgnore]
    public bool EmailSpecified { get; private init; }

    [JsonIgnore]
    public bool RtnSpecified { get; private init; }

    [JsonIgnore]
    public bool PhoneNumberSpecified { get; private init; }

    [JsonIgnore]
    public bool ActiveSpecified { get; private init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!NameSpecified && !EmailSpecified && !RtnSpecified &&
            !PhoneNumberSpecified && !ActiveSpecified)
        {
            yield return new ValidationResult(
                "Debe especificar al menos un campo para actualizar.");
            yield break;
        }

        if (NameSpecified && (string.IsNullOrWhiteSpace(Name) || Name.Length > 100))
        {
            yield return new ValidationResult(
                "El nombre debe contener entre 1 y 100 caracteres.",
                new[] { nameof(Name) });
        }

        if (EmailSpecified && Email is not null &&
            (string.IsNullOrWhiteSpace(Email) ||
             !new EmailAddressAttribute().IsValid(Email)))
        {
            yield return new ValidationResult(
                "El correo electrónico no tiene un formato válido.",
                new[] { nameof(Email) });
        }

        if (RtnSpecified && Rtn is not null && string.IsNullOrWhiteSpace(Rtn))
        {
            yield return new ValidationResult(
                "El RTN no puede estar vacío; envíe null para limpiarlo.",
                new[] { nameof(Rtn) });
        }

        if (PhoneNumberSpecified && PhoneNumber is not null &&
            !System.Text.RegularExpressions.Regex.IsMatch(PhoneNumber, @"^\d{8}$"))
        {
            yield return new ValidationResult(
                "El teléfono debe contener exactamente 8 dígitos.",
                new[] { nameof(PhoneNumber) });
        }

        if (ActiveSpecified && !Active.HasValue)
        {
            yield return new ValidationResult(
                "El estado activo debe ser true o false.",
                new[] { nameof(Active) });
        }
    }
}
