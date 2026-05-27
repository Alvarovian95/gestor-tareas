namespace GestorTareas.Domain.Exceptions;
public sealed class InvalidCategoryException : DomainException
{
    public InvalidCategoryException(string message) : base(message) { }
    public InvalidCategoryException(string message, Exception innerException) : base(message, innerException) { }
    public static InvalidCategoryException EmptyName() => new("El nombre de la categoría no puede estar vacío.");
    public static InvalidCategoryException NameTooLong(int maxLength) => new($"El nombre de la categoría no puede tener más de {maxLength} caracteres.");
    public static InvalidCategoryException InvalidColor(string color) => new($"El color '{color}' no es un código hexadecimal válido (esperado: #RRGGBB).");
    public static InvalidCategoryException MissingOwner() => new("La categoría debe tener un usuario propietario.");
}