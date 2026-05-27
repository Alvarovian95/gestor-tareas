namespace GestorTareas.Domain.Exceptions;
public sealed class InvalidUserException : DomainException
{
    public InvalidUserException(string message) : base(message) { }
    public InvalidUserException(string message, Exception innerException) : base(message, innerException) { }

    public static InvalidUserException EmptyName() => new("El nombre del usuario no puede estar vacío.");
    public static InvalidUserException NameTooLong(int maxLength) => new($"El nombre del usuario no puede tener más de {maxLength} caracteres.");
    public static InvalidUserException InvalidPasswordHash() => new("La contraseña proporcionada no es válida.");
}