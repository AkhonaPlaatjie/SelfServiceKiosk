using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SelfServiceKiosk.Api.Data;
using SelfServiceKiosk.Api.Exceptions;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly KioskDbContext _context;

    public CategoryRepository(KioskDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Category> Items, int TotalCount)> GetPageAsync(
        int page,
        int pageSize,
        bool activeOnly,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.AsNoTracking();
        if (activeOnly)
        {
            query = query.Where(category => category.IsActive);
        }

        return await query.OrderBy(category => category.Name).ToPageAsync(page, pageSize, cancellationToken);
    }

    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Categories.SingleOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default)
    {
        var trimmed = name.Trim();

        return await _context.Categories.AnyAsync(
            category => category.Name == trimmed && (excludeId == null || category.Id != excludeId),
            cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Add(category);
        await SaveAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveAsync(cancellationToken);
    }

    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new ConflictException("A category with this name already exists.");
        }
    }
}
