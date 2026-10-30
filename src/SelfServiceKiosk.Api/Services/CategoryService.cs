using SelfServiceKiosk.Api.DTOs.Categories;
using SelfServiceKiosk.Api.DTOs.Common;
using SelfServiceKiosk.Api.Exceptions;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Mappers;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Services;

public class CategoryService : ICategoryService
{
    private const string NotFoundMessage = "Category not found.";
    private const string DuplicateMessage = "A category with this name already exists.";

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

    public async Task<PagedResult<CategoryDto>> GetPageAsync(
        PageQuery query,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _categoryRepository.GetPageAsync(
            query.Page,
            query.PageSize,
            activeOnly: !includeInactive,
            cancellationToken);

        return new PagedResult<CategoryDto>
        {
            Items = items.Select(category => category.ToDto()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total,
        };
    }

    public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);

        return category.ToDto();
    }

    public async Task<CategoryDto> CreateAsync(SaveCategoryRequestDto request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        if (await _categoryRepository.NameExistsAsync(name, null, cancellationToken))
        {
            throw new ConflictException(DuplicateMessage);
        }

        var category = new Category { Name = name, IsActive = true };
        await _categoryRepository.AddAsync(category, cancellationToken);

        return category.ToDto();
    }

    public async Task<CategoryDto> UpdateAsync(int id, SaveCategoryRequestDto request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);

        var name = request.Name.Trim();
        if (await _categoryRepository.NameExistsAsync(name, id, cancellationToken))
        {
            throw new ConflictException(DuplicateMessage);
        }

        category.Name = name;
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return category.ToDto();
    }

    public async Task<CategoryDto> SetActiveAsync(int id, bool isActive, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);

        category.IsActive = isActive; // soft delete: the row stays
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return category.ToDto();
    }
}