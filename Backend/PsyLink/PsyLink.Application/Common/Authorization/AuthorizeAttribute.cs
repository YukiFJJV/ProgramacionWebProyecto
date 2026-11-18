namespace PsyLink.Application.Common.Authorization;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class AuthorizeAttribute(params string[] roles) : Attribute
{
    public IReadOnlyCollection<string> Roles { get; } = roles;
}
