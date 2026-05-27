namespace GestorTareas.Domain.Exceptions;
public sealed class InvalidTaskItemException : DomainException
{
    public InvalidTaskItemException(string message) : base(message) { }
    public InvalidTaskItemException(string message, Exception innerException) : base(message, innerException) { }

    public static InvalidTaskItemException EmptyTitle() => new("El título de la tarea no puede estar vacío.");
    public static InvalidTaskItemException TitleTooLong(int maxLength) => new($"El título no puede tener más de {maxLength} caracteres.");
    public static InvalidTaskItemException DescriptionTooLong(int maxLength) => new($"La descripción no puede tener más de {maxLength} caracteres.");
    public static InvalidTaskItemException DueDateInPast() => new("La fecha límite no puede ser en el pasado.");
    public static InvalidTaskItemException MissingOwner() => new("La tarea debe tener un usuario propietario.");
    public static InvalidTaskItemException AlreadyCompleted() => new("La tarea ya está completada.");
    public static InvalidTaskItemException CannotReopenNonCompleted() => new("Solo se pueden reabrir tareas que están completadas.");
    public static InvalidTaskItemException CannotStartFromCurrentState() => new("Solo se pueden iniciar tareas que están en estado pendiente.");
}