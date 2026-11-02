using SelfServiceKiosk.Api.DTOs.Products;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQty = product.StockQty,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            IsActive = product.IsActive,
            IsAvailable = product.IsAvailable,
        };
    }
}
