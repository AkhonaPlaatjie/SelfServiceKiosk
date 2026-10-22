using SelfServiceKiosk.Api.DTOs.Categories;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Mappers;

namespace SelfServiceKiosk.Api.Services;

/// <summary>Business rules for categories. It can be unit tested without a database or a web server.</summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);

        return categories.Select(category => category.ToDto()).ToList();
    }
}
