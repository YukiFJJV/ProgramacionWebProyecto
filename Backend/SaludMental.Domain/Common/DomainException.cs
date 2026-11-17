namespace SaludMental.Domain.Common;

/// <summary>
/// Se lanza cuando se viola una regla de negocio del dominio.
/// La capa API la traduce a un HTTP 400/409.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
