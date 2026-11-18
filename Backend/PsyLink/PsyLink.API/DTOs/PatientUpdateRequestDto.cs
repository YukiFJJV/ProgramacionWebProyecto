using System.Text.Json.Serialization;

namespace PsyLink.API.DTOs;

/// <summary>Optional fields that a psychologist may update on a patient profile.</summary>
public sealed record PatientUpdateRequestDto
{
    private string? _firstName;
    private string? _lastName;
    private string? _email;
    private string? _phoneNumber;
    private DateOnly? _birthDate;
    private string? _description;

    public string? FirstName
    {
        get => _firstName;
        init
        {
            _firstName = value;
            FirstNameSpecified = true;
        }
    }

    public string? LastName
    {
        get => _lastName;
        init
        {
            _lastName = value;
            LastNameSpecified = true;
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

    public string? PhoneNumber
    {
        get => _phoneNumber;
        init
        {
            _phoneNumber = value;
            PhoneNumberSpecified = true;
        }
    }

    public DateOnly? BirthDate
    {
        get => _birthDate;
        init
        {
            _birthDate = value;
            BirthDateSpecified = true;
        }
    }

    public string? Description
    {
        get => _description;
        init
        {
            _description = value;
            DescriptionSpecified = true;
        }
    }

    [JsonIgnore]
    public bool FirstNameSpecified { get; private init; }

    [JsonIgnore]
    public bool LastNameSpecified { get; private init; }

    [JsonIgnore]
    public bool EmailSpecified { get; private init; }

    [JsonIgnore]
    public bool PhoneNumberSpecified { get; private init; }

    [JsonIgnore]
    public bool BirthDateSpecified { get; private init; }

    [JsonIgnore]
    public bool DescriptionSpecified { get; private init; }
}
