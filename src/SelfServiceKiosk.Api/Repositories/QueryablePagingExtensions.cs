using Microsoft.EntityFrameworkCore;

namespace SelfServiceKiosk.Api.Repositories;

public static class QueryablePagingExtensions
{
    /// <summary>Counts the rows, then returns one page. The query must already be ordered.</summary>
    public static async Task<(IReadOnlyList<T> Items, int TotalCount)> ToPageAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, total);
    }
}
