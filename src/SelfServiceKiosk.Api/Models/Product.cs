namespace SelfServiceKiosk.Api.Models;

/// <summary>An item the kiosk sells.</summary>
public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQty { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    /// <summary>False means deleted (a soft delete, BR-10).</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>False hides the product from customers without deleting it.</summary>
    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }
}
