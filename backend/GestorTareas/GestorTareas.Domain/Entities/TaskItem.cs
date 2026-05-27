using GestorTareas.Domain.Common;
using GestorTareas.Domain.Enums;
using GestorTareas.Domain.Exceptions;

namespace GestorTareas.Domain.Entities;

public sealed class TaskItem : Entity
{
    private const int MaxTitleLength = 200;
    private const int MaxDescriptionLength = 2000;
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TaskItemStatus Status { get; private set; }
    public Priority Priority { get; private set; }
    public DateTime? DueDate { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private TaskItem(
        Guid id,
        string title,
        string? description,
        TaskItemStatus status,
        Priority priority,
        DateTime? dueDate,
        Guid userId,
        Guid? categoryId,
        DateTime createdAt,
        DateTime? completedAt)
        : base(id)
    {
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        UserId = userId;
        CategoryId = categoryId;
        CreatedAt = createdAt;
        CompletedAt = completedAt;
    }

    private TaskItem() : base()
    {
        Title = null!;
    }

    public static TaskItem Create(
        string title,
        Guid userId,
        string? description = null,
        Priority priority = Priority.Medium,
        DateTime? dueDate = null,
        Guid? categoryId = null)
    {
        ValidateTitle(title);
        ValidateDescription(description);
        ValidateUserId(userId);
        ValidateDueDate(dueDate);

        return new TaskItem(
            id: Guid.NewGuid(),
            title: title.Trim(),
            description: description?.Trim(),
            status: TaskItemStatus.Pending,
            priority: priority,
            dueDate: dueDate,
            userId: userId,
            categoryId: categoryId,
            createdAt: DateTime.UtcNow,
            completedAt: null);
    }

    public void UpdateDetails(string newTitle, string? newDescription)
    {
        ValidateTitle(newTitle);
        ValidateDescription(newDescription);

        Title = newTitle.Trim();
        Description = newDescription?.Trim();
    }
    public void ChangePriority(Priority newPriority)
    {
        if (Priority == newPriority)
            return;

        Priority = newPriority;
    }

    public void ChangeDueDate(DateTime? newDueDate)
    {
        ValidateDueDate(newDueDate);
        DueDate = newDueDate;
    }
    public void AssignToCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new InvalidTaskItemException("La categoría no es válida.");

        CategoryId = categoryId;
    }
    public void RemoveFromCategory()
    {
        CategoryId = null;
    }
    public void Start()
    {
        if (Status != TaskItemStatus.Pending)
            throw InvalidTaskItemException.CannotStartFromCurrentState();

        Status = TaskItemStatus.InProgress;
    }

    public void MarkAsCompleted()
    {
        if (Status == TaskItemStatus.Completed)
            throw InvalidTaskItemException.AlreadyCompleted();

        Status = TaskItemStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Reopen()
    {
        if (Status != TaskItemStatus.Completed)
            throw InvalidTaskItemException.CannotReopenNonCompleted();

        Status = TaskItemStatus.Pending;
        CompletedAt = null;
    }

    public bool IsOverdue()
    {
        return DueDate.HasValue
            && DueDate.Value < DateTime.UtcNow
            && Status != TaskItemStatus.Completed;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw InvalidTaskItemException.EmptyTitle();

        if (title.Trim().Length > MaxTitleLength)
            throw InvalidTaskItemException.TitleTooLong(MaxTitleLength);
    }

    private static void ValidateDescription(string? description)
    {
        if (description is not null && description.Trim().Length > MaxDescriptionLength)
            throw InvalidTaskItemException.DescriptionTooLong(MaxDescriptionLength);
    }

    private static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw InvalidTaskItemException.MissingOwner();
    }

    private static void ValidateDueDate(DateTime? dueDate)
    {
        if (dueDate.HasValue && dueDate.Value < DateTime.UtcNow)
            throw InvalidTaskItemException.DueDateInPast();
    }
}