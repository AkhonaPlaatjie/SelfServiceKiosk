using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Interfaces;

public interface IProductRepository
{
    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPageAsync(
        int page,
        int pageSize,
        int? categoryId,
        bool? isAvailable,
        bool customersOnly,
        CancellationToken cancellationToken = default);

    /// <summary>Finds a product for changing it (tracked), or returns null.</summary>
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
