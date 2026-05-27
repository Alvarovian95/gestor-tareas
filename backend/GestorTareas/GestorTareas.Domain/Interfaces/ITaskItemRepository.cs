using GestorTareas.Domain.Common;
using GestorTareas.Domain.Entities;
using GestorTareas.Domain.Enums;

namespace GestorTareas.Domain.Interfaces;
public interface ITaskItemRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResult<TaskItem>> GetPagedForUserAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        TaskItemStatus? status = null,
        Priority? priority = null,
        Guid? categoryId = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<int> CountByStatusForUserAsync(Guid userId, TaskItemStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    void Update(TaskItem task);
    void Delete(TaskItem task);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}