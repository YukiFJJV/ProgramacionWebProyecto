namespace PsyLink.Application.Features.Users.DTOs;

public sealed record UserUpdateRequestDto
{
    private string? _email;
    private string? _firstName;
    private string? _lastName;
    private string? _phoneNumber;
    private Guid? _roleId;
    private bool? _active;

    public string? Email
    {
        get => _email;
        init { _email = value; EmailSpecified = true; }
    }

    public string? FirstName
    {
        get => _firstName;
        init { _firstName = value; FirstNameSpecified = true; }
    }

    public string? LastName
    {
        get => _lastName;
        init { _lastName = value; LastNameSpecified = true; }
    }

    public string? PhoneNumber
    {
        get => _phoneNumber;
        init { _phoneNumber = value; PhoneNumberSpecified = true; }
    }

    public Guid? RoleId
    {
        get => _roleId;
        init { _roleId = value; RoleIdSpecified = true; }
    }

    public bool? Active
    {
        get => _active;
        init { _active = value; ActiveSpecified = true; }
    }

    public bool EmailSpecified { get; private init; }
    public bool FirstNameSpecified { get; private init; }
    public bool LastNameSpecified { get; private init; }
    public bool PhoneNumberSpecified { get; private init; }
    public bool RoleIdSpecified { get; private init; }
    public bool ActiveSpecified { get; private init; }
}
