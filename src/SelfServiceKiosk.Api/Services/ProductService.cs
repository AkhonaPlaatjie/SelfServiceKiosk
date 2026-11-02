using SelfServiceKiosk.Api.Constants;
using SelfServiceKiosk.Api.DTOs.Common;
using SelfServiceKiosk.Api.DTOs.Products;
using SelfServiceKiosk.Api.Exceptions;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Mappers;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Services;

public class ProductService : IProductService
{
    private const string NotFoundMessage = "Product not found.";

    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<ProductDto>> GetPageAsync(
        PageQuery query,
        int? categoryId,
        bool? isAvailable,
        bool isAdministrator,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _productRepository.GetPageAsync(
            query.Page,
            query.PageSize,
            categoryId,
            isAvailable,
            !isAdministrator,
            cancellationToken);

        return new PagedResult<ProductDto>
        {
            Items = items.Select(product => product.ToDto()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total,
        };
    }

    public async Task<ProductDto> GetByIdAsync(int id, bool isAdministrator, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null || (!isAdministrator && (!product.IsActive || !product.IsAvailable)))
        {
            throw new NotFoundException(NotFoundMessage);
        }

        return product.ToDto();
    }

    public async Task<ProductDto> CreateAsync(SaveProductRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureCategoryIsUsableAsync(request.CategoryId, cancellationToken);

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = CleanOptional(request.Description),
            Price = request.Price,
            StockQty = 0,
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl)
                ? ProductDefaults.PlaceholderImageUrl
                : request.ImageUrl.Trim(),
            CategoryId = request.CategoryId,
            IsActive = true,
            IsAvailable = request.IsAvailable,
            CreatedAtUtc = DateTime.UtcNow,
        };

        await _productRepository.AddAsync(product, cancellationToken);

        return product.ToDto();
    }

    public async Task<ProductDto> UpdateAsync(int id, SaveProductRequestDto request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);

        await EnsureCategoryIsUsableAsync(request.CategoryId, cancellationToken);

        product.Name = request.Name.Trim();
        product.Description = CleanOptional(request.Description);
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;
        product.IsAvailable = request.IsAvailable;

        // An update without an image keeps the current one.
        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            product.ImageUrl = request.ImageUrl.Trim();
        }

        await _productRepository.SaveChangesAsync(cancellationToken);

        return product.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);

        product.IsActive = false; // soft delete (BR-10): the row stays
        await _productRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCategoryIsUsableAsync(int categoryId, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null || !category.IsActive)
        {
            throw new BadRequestException("Category does not exist or is not active.");
        }
    }

    private static string? CleanOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
