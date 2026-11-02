namespace SelfServiceKiosk.Api.DTOs.Products;

/// <summary>A product as the client sees it.</summary>
public class ProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQty { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public bool IsActive { get; set; }

    public bool IsAvailable { get; set; }
}
