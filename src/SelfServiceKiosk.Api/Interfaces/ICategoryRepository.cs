using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Interfaces;

/// <summary>Data access for categories. Services depend on this interface, not on EF Core.</summary>
public interface ICategoryRepository
{
    /// <summary>Returns every category, ordered by name.</summary>
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
}
