using Microsoft.EntityFrameworkCore;
using SelfServiceKiosk.Api.Data;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Repositories;

public class StockRepository : IStockRepository
{
    private readonly KioskDbContext _context;

    public StockRepository(KioskDbContext context)
    {
        _context = context;
    }

    public Task<Product?> GetProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        return _context.Products.SingleOrDefaultAsync(product => product.Id == productId, cancellationToken);
    }

    public Task<StockPurchase?> GetPurchaseAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.StockPurchases
            .AsNoTracking()
            .SingleOrDefaultAsync(purchase => purchase.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<StockPurchase> Items, int TotalCount)> GetPageAsync(
        int page,
        int pageSize,
        int? productId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        IQueryable<StockPurchase> query = _context.StockPurchases.AsNoTracking();

        if (productId is not null)
        {
            query = query.Where(purchase => purchase.ProductId == productId);
        }

        if (fromUtc is not null)
        {
            query = query.Where(purchase => purchase.PurchaseDateUtc >= fromUtc);
        }

        if (toUtc is not null)
        {
            query = query.Where(purchase => purchase.PurchaseDateUtc <= toUtc);
        }

        return await query
            .OrderByDescending(purchase => purchase.PurchaseDateUtc)
            .ThenByDescending(purchase => purchase.Id)
            .ToPageAsync(page, pageSize, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(ProductFilters.LowStock)
            .OrderBy(product => product.StockQty)
            .ThenBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddPurchaseAsync(StockPurchase purchase, CancellationToken cancellationToken = default)
    {
        _context.StockPurchases.Add(purchase);

        // One SaveChanges is one database transaction. The product's new stock level was changed on the tracked
        // product by the service, so the purchase and the stock change are saved together or not at all.
        await _context.SaveChangesAsync(cancellationToken);
    }
}
