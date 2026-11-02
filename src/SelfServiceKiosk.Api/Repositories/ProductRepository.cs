using Microsoft.EntityFrameworkCore;
using SelfServiceKiosk.Api.Data;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly KioskDbContext _context;

    public ProductRepository(KioskDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPageAsync(
        int page,
        int pageSize,
        int? categoryId,
        bool? isAvailable,
        bool customersOnly,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _context.Products.AsNoTracking();

        if (customersOnly)
        {
            query = query.Where(product => product.IsActive && product.IsAvailable);
        }

        if (categoryId is not null)
        {
            query = query.Where(product => product.CategoryId == categoryId);
        }

        if (isAvailable is not null)
        {
            query = query.Where(product => product.IsAvailable == isAvailable);
        }

        return await query
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Id)
            .ToPageAsync(page, pageSize, cancellationToken);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Products.SingleOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
