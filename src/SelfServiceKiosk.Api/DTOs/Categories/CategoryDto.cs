namespace SelfServiceKiosk.Api.DTOs.Categories;

/// <summary>A product category as the client sees it.</summary>
public class CategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
