using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Interfaces;

public interface IStockRepository
{
    /// <summary>Finds a product for changing its stock (tracked), or returns null.</summary>
    Task<Product?> GetProductAsync(int productId, CancellationToken cancellationToken = default);

    Task<StockPurchase?> GetPurchaseAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<StockPurchase> Items, int TotalCount)> GetPageAsync(
        int page,
        int pageSize,
        int? productId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetLowStockAsync(CancellationToken cancellationToken = default);

    /// <summary>Saves the purchase together with the product's changed stock level, in one database transaction.</summary>
    Task AddPurchaseAsync(StockPurchase purchase, CancellationToken cancellationToken = default);
}
