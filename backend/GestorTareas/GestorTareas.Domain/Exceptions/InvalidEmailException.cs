namespace GestorTareas.Domain.Exceptions;
public sealed class InvalidEmailException : DomainException
{
    public InvalidEmailException(string message) : base(message)
    {
    }
    public InvalidEmailException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public static InvalidEmailException InvalidFormat(string value) => new($"El email '{value}' no tiene un formato válido.");
    public static InvalidEmailException Empty() => new("El email no puede estar vacío.");
}