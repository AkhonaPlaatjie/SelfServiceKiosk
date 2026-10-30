using SelfServiceKiosk.Api.DTOs.Categories;
using SelfServiceKiosk.Api.DTOs.Common;

namespace SelfServiceKiosk.Api.Interfaces;

public interface ICategoryService
{
    /// <summary>Active categories only, for the kiosk.</summary>
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<CategoryDto>> GetPageAsync(PageQuery query, bool includeInactive, CancellationToken cancellationToken = default);

    Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CategoryDto> CreateAsync(SaveCategoryRequestDto request, CancellationToken cancellationToken = default);

    Task<CategoryDto> UpdateAsync(int id, SaveCategoryRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>Deactivates or reactivates a category. Nothing is ever deleted.</summary>
    Task<CategoryDto> SetActiveAsync(int id, bool isActive, CancellationToken cancellationToken = default);
}
