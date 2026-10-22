using Microsoft.EntityFrameworkCore;
using SelfServiceKiosk.Api.Data;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Models;
using SelfServiceKiosk.Api.Interfaces;

namespace SelfServiceKiosk.Api.Repositories;

/// <summary>The only place that queries the Categories table.</summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly KioskDbContext _context;

    public CategoryRepository(KioskDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // AsNoTracking: this is a read-only query, so EF Core does not need to track the entities.
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }
}
