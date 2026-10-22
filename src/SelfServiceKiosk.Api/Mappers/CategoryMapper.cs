using SelfServiceKiosk.Api.DTOs.Categories;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Mappers;

/// <summary>Converts Category entities into DTOs.</summary>
public static class CategoryMapper
{
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
        };
    }
}
