using Microsoft.EntityFrameworkCore;
using SelfServiceKiosk.Api.Models;

public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPageAsync(
    int page, int pageSize, int? categoryId, bool? isAvailable, bool customersOnly,
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

    return await query.OrderBy(product => product.Name).ToPageAsync(page, pageSize, cancellationToken);
}
