using SelfServiceKiosk.Api.DTOs.Categories;

namespace SelfServiceKiosk.Api.Interfaces;

/// <summary>Business rules for categories.</summary>
public interface ICategoryService
{
    /// <summary>Returns every category as a DTO, ordered by name.</summary>
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
