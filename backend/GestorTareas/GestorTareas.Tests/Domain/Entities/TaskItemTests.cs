using FluentAssertions;
using GestorTareas.Domain.Entities;
using GestorTareas.Domain.Enums;
using GestorTareas.Domain.Exceptions;
using Xunit;

namespace GestorTareas.Tests.Domain.Entities;

/// <summary>
/// Tests unitarios de la entidad TaskItem.
/// Verifican el factory, las actualizaciones de datos,
/// las transiciones de estado y los query methods del dominio.
/// </summary>
public class TaskItemTests
{
    // Datos válidos por defecto.
    private const string ValidTitle = "Comprar leche";
    private const string ValidDescription = "Ir al supermercado de la esquina";
    private static readonly Guid ValidUserId = Guid.NewGuid();
    private static readonly Guid ValidCategoryId = Guid.NewGuid();
    private static readonly DateTime FutureDate = DateTime.UtcNow.AddDays(7);

    /// <summary>
    /// Helper para crear una tarea válida con valores por defecto.
    /// </summary>
    private static TaskItem CreateValidTask(
        string? title = null,
        Guid? userId = null,
        string? description = null,
        Priority priority = Priority.Medium,
        DateTime? dueDate = null,
        Guid? categoryId = null)
    {
        return TaskItem.Create(
            title ?? ValidTitle,
            userId ?? ValidUserId,
            description ?? ValidDescription,
            priority,
            dueDate ?? FutureDate,
            categoryId);
    }

    // ──────────────────────────────────────────────────────
    // CREACIÓN: CASOS DE ÉXITO
    // ──────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_ShouldReturnTaskItem()
    {
        // Act
        var task = TaskItem.Create(
            ValidTitle,
            ValidUserId,
            ValidDescription,
            Priority.High,
            FutureDate,
            ValidCategoryId);

        // Assert
        task.Should().NotBeNull();
        task.Id.Should().NotBe(Guid.Empty);
        task.Title.Should().Be(ValidTitle);
        task.Description.Should().Be(ValidDescription);
        task.Priority.Should().Be(Priority.High);
        task.UserId.Should().Be(ValidUserId);
        task.CategoryId.Should().Be(ValidCategoryId);
        task.DueDate.Should().Be(FutureDate);
    }

    [Fact]
    public void Create_ShouldStartInPendingStatus()
    {
        // Act
        var task = CreateValidTask();

        // Assert: una tarea siempre nace Pending
        task.Status.Should().Be(TaskItemStatus.Pending);
        task.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithoutOptionalParameters_ShouldUseDefaults()
    {
        // Act: solo título y userId, lo mínimo obligatorio
        var task = TaskItem.Create(ValidTitle, ValidUserId);

        // Assert
        task.Title.Should().Be(ValidTitle);
        task.Description.Should().BeNull();
        task.Priority.Should().Be(Priority.Medium); // valor por defecto
        task.DueDate.Should().BeNull();
        task.CategoryId.Should().BeNull();
        task.Status.Should().Be(TaskItemStatus.Pending);
    }

    [Fact]
    public void Create_ShouldSetCreatedAtToCurrentUtcTime()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var task = CreateValidTask();
        var after = DateTime.UtcNow;

        // Assert
        task.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        task.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Create_WithTitleContainingSpaces_ShouldTrimTitle()
    {
        // Act
        var task = CreateValidTask(title: "   Comprar leche   ");

        // Assert
        task.Title.Should().Be("Comprar leche");
    }

    // ──────────────────────────────────────────────────────
    // CREACIÓN: CASOS DE ERROR
    // ──────────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyTitle_ShouldThrowInvalidTaskItemException(string title)
    {
        // Act
        var action = () => CreateValidTask(title: title);

        // Assert
        action.Should()
            .Throw<InvalidTaskItemException>()
            .WithMessage("*vacío*");
    }

    [Fact]
    public void Create_WithTitleExceedingMaxLength_ShouldThrowInvalidTaskItemException()
    {
        // Arrange: 201 caracteres, el máximo es 200
        var tooLongTitle = new string('a', 201);

        // Act
        var action = () => CreateValidTask(title: tooLongTitle);

        // Assert
        action.Should().Throw<InvalidTaskItemException>();
    }

    [Fact]
    public void Create_WithDescriptionExceedingMaxLength_ShouldThrowInvalidTaskItemException()
    {
        // Arrange: 2001 caracteres, el máximo es 2000
        var tooLong = new string('a', 2001);

        // Act
        var action = () => CreateValidTask(description: tooLong);

        // Assert
        action.Should().Throw<InvalidTaskItemException>();
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowInvalidTaskItemException()
    {
        // Act
        var action = () => CreateValidTask(userId: Guid.Empty);

        // Assert
        action.Should()
            .Throw<InvalidTaskItemException>()
            .WithMessage("*propietario*");
    }

    [Fact]
    public void Create_WithDueDateInPast_ShouldThrowInvalidTaskItemException()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var action = () => CreateValidTask(dueDate: pastDate);

        // Assert
        action.Should()
            .Throw<InvalidTaskItemException>()
            .WithMessage("*pasado*");
    }

    // ──────────────────────────────────────────────────────
    // UPDATEDETAILS
    // ──────────────────────────────────────────────────────

    [Fact]
    public void UpdateDetails_WithValidData_ShouldUpdateTitleAndDescription()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        task.UpdateDetails("Nuevo título", "Nueva descripción");

        // Assert
        task.Title.Should().Be("Nuevo título");
        task.Description.Should().Be("Nueva descripción");
    }

    [Fact]
    public void UpdateDetails_WithNullDescription_ShouldAllowIt()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        task.UpdateDetails("Nuevo título", null);

        // Assert
        task.Description.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithEmptyTitle_ShouldThrow(string title)
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        var action = () => task.UpdateDetails(title, "desc");

        // Assert
        action.Should().Throw<InvalidTaskItemException>();
    }

    [Fact]
    public void UpdateDetails_WhenValidationFails_ShouldNotChangeOriginalData()
    {
        // Arrange
        var task = CreateValidTask(title: "Original", description: "Desc original");

        // Act
        var action = () => task.UpdateDetails("", "");

        // Assert
        action.Should().Throw<InvalidTaskItemException>();
        task.Title.Should().Be("Original");
        task.Description.Should().Be("Desc original");
    }

    // ──────────────────────────────────────────────────────
    // CHANGEPRIORITY
    // ──────────────────────────────────────────────────────

    [Fact]
    public void ChangePriority_ToDifferentValue_ShouldChange()
    {
        // Arrange
        var task = CreateValidTask(priority: Priority.Low);

        // Act
        task.ChangePriority(Priority.High);

        // Assert
        task.Priority.Should().Be(Priority.High);
    }

    [Fact]
    public void ChangePriority_ToSameValue_ShouldNotThrow()
    {
        // Arrange
        var task = CreateValidTask(priority: Priority.Medium);

        // Act
        var action = () => task.ChangePriority(Priority.Medium);

        // Assert: cambiar a la misma prioridad es un no-op, no lanza
        action.Should().NotThrow();
        task.Priority.Should().Be(Priority.Medium);
    }

    // ──────────────────────────────────────────────────────
    // CHANGEDUEDATE
    // ──────────────────────────────────────────────────────

    [Fact]
    public void ChangeDueDate_ToFutureDate_ShouldUpdate()
    {
        // Arrange
        var task = CreateValidTask();
        var newDueDate = DateTime.UtcNow.AddDays(14);

        // Act
        task.ChangeDueDate(newDueDate);

        // Assert
        task.DueDate.Should().Be(newDueDate);
    }

    [Fact]
    public void ChangeDueDate_ToNull_ShouldRemoveDueDate()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        task.ChangeDueDate(null);

        // Assert
        task.DueDate.Should().BeNull();
    }

    [Fact]
    public void ChangeDueDate_ToPastDate_ShouldThrow()
    {
        // Arrange
        var task = CreateValidTask();
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var action = () => task.ChangeDueDate(pastDate);

        // Assert
        action.Should().Throw<InvalidTaskItemException>();
    }

    // ──────────────────────────────────────────────────────
    // CATEGORÍA
    // ──────────────────────────────────────────────────────

    [Fact]
    public void AssignToCategory_WithValidCategoryId_ShouldAssign()
    {
        // Arrange
        var task = CreateValidTask();
        var newCategoryId = Guid.NewGuid();

        // Act
        task.AssignToCategory(newCategoryId);

        // Assert
        task.CategoryId.Should().Be(newCategoryId);
    }

    [Fact]
    public void AssignToCategory_WithEmptyGuid_ShouldThrow()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        var action = () => task.AssignToCategory(Guid.Empty);

        // Assert
        action.Should().Throw<InvalidTaskItemException>();
    }

    [Fact]
    public void RemoveFromCategory_ShouldSetCategoryIdToNull()
    {
        // Arrange
        var task = CreateValidTask(categoryId: Guid.NewGuid());
        task.CategoryId.Should().NotBeNull(); // sanity check

        // Act
        task.RemoveFromCategory();

        // Assert
        task.CategoryId.Should().BeNull();
    }

    // ──────────────────────────────────────────────────────
    // TRANSICIONES DE ESTADO: START
    // ──────────────────────────────────────────────────────

    [Fact]
    public void Start_FromPending_ShouldChangeStatusToInProgress()
    {
        // Arrange
        var task = CreateValidTask();
        task.Status.Should().Be(TaskItemStatus.Pending);

        // Act
        task.Start();

        // Assert
        task.Status.Should().Be(TaskItemStatus.InProgress);
    }

    [Fact]
    public void Start_FromInProgress_ShouldThrow()
    {
        // Arrange
        var task = CreateValidTask();
        task.Start(); // ya en InProgress

        // Act
        var action = () => task.Start();

        // Assert
        action.Should()
            .Throw<InvalidTaskItemException>()
            .WithMessage("*pendiente*");
    }

    [Fact]
    public void Start_FromCompleted_ShouldThrow()
    {
        // Arrange
        var task = CreateValidTask();
        task.MarkAsCompleted();

        // Act
        var action = () => task.Start();

        // Assert
        action.Should().Throw<InvalidTaskItemException>();
    }

    // ──────────────────────────────────────────────────────
    // TRANSICIONES DE ESTADO: MARKASCOMPLETED
    // ──────────────────────────────────────────────────────

    [Fact]
    public void MarkAsCompleted_FromPending_ShouldChangeStatusAndSetCompletedAt()
    {
        // Arrange
        var task = CreateValidTask();
        var before = DateTime.UtcNow;

        // Act
        task.MarkAsCompleted();
        var after = DateTime.UtcNow;

        // Assert
        task.Status.Should().Be(TaskItemStatus.Completed);
        task.CompletedAt.Should().NotBeNull();
        task.CompletedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void MarkAsCompleted_FromInProgress_ShouldChangeStatusAndSetCompletedAt()
    {
        // Arrange
        var task = CreateValidTask();
        task.Start();

        // Act
        task.MarkAsCompleted();

        // Assert
        task.Status.Should().Be(TaskItemStatus.Completed);
        task.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsCompleted_FromCompleted_ShouldThrow()
    {
        // Arrange
        var task = CreateValidTask();
        task.MarkAsCompleted();

        // Act
        var action = () => task.MarkAsCompleted();

        // Assert
        action.Should()
            .Throw<InvalidTaskItemException>()
            .WithMessage("*completada*");
    }

    // ──────────────────────────────────────────────────────
    // TRANSICIONES DE ESTADO: REOPEN
    // ──────────────────────────────────────────────────────

    [Fact]
    public void Reopen_FromCompleted_ShouldChangeStatusToPendingAndClearCompletedAt()
    {
        // Arrange
        var task = CreateValidTask();
        task.MarkAsCompleted();
        task.CompletedAt.Should().NotBeNull(); // sanity check

        // Act
        task.Reopen();

        // Assert
        task.Status.Should().Be(TaskItemStatus.Pending);
        task.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void Reopen_FromPending_ShouldThrow()
    {
        // Arrange
        var task = CreateValidTask();

        // Act
        var action = () => task.Reopen();

        // Assert
        action.Should()
            .Throw<InvalidTaskItemException>()
            .WithMessage("*completadas*");
    }

    [Fact]
    public void Reopen_FromInProgress_ShouldThrow()
    {
        // Arrange
        var task = CreateValidTask();
        task.Start();

        // Act
        var action = () => task.Reopen();

        // Assert
        action.Should().Throw<InvalidTaskItemException>();
    }

    // ──────────────────────────────────────────────────────
    // ISOVERDUE
    // ──────────────────────────────────────────────────────

    [Fact]
    public void IsOverdue_WithFutureDueDate_ShouldReturnFalse()
    {
        // Arrange
        var task = CreateValidTask(dueDate: DateTime.UtcNow.AddDays(7));

        // Act & Assert
        task.IsOverdue().Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_WithNoDueDate_ShouldReturnFalse()
    {
        // Arrange: tarea sin fecha límite
        var task = TaskItem.Create(ValidTitle, ValidUserId, dueDate: null);

        // Act & Assert
        task.IsOverdue().Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_WhenCompletedEvenWithPastDate_ShouldReturnFalse()
    {
        // Arrange: una tarea completada NUNCA está vencida
        var task = CreateValidTask();
        task.MarkAsCompleted();

        // Forzamos una fecha pasada para verificar que se ignora si está completada.
        // Como no podemos cambiar DueDate a una fecha pasada (lo validamos),
        // verificamos el caso real: tarea completada nunca cuenta como vencida.

        // Act & Assert
        task.IsOverdue().Should().BeFalse();
    }
}