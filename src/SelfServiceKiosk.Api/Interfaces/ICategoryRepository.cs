using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Interfaces;

public interface ICategoryRepository
{
    /// <summary>Returns the active categories, ordered by name.</summary>
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Category> Items, int TotalCount)> GetPageAsync(
        int page,
        int pageSize,
        bool activeOnly,
        CancellationToken cancellationToken = default);

    /// <summary>Finds a category for changing it (tracked), or returns null.</summary>
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default);

    Task AddAsync(Category category, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
