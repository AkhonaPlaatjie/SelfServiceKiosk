using SelfServiceKiosk.Api.DTOs.Common;
using SelfServiceKiosk.Api.DTOs.Products;

namespace SelfServiceKiosk.Api.Interfaces;

public interface IProductService
{
    /// <summary>Customers only see products that are active and available; administrators see everything.</summary>
    Task<PagedResult<ProductDto>> GetPageAsync(
        PageQuery query,
        int? categoryId,
        bool? isAvailable,
        bool isAdministrator,
        CancellationToken cancellationToken = default);

    Task<ProductDto> GetByIdAsync(int id, bool isAdministrator, CancellationToken cancellationToken = default);

    Task<ProductDto> CreateAsync(SaveProductRequestDto request, CancellationToken cancellationToken = default);

    Task<ProductDto> UpdateAsync(int id, SaveProductRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>A soft delete (BR-10): sets IsActive to false. The row is never removed.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
