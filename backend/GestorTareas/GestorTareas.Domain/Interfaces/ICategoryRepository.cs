using GestorTareas.Domain.Entities;

namespace GestorTareas.Domain.Interfaces;
public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameForUserAsync(Guid userId, string name, Guid? excludeCategoryId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    void Update(Category category);
    void Delete(Category category);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}