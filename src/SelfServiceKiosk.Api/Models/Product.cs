namespace SelfServiceKiosk.Api.Models;

/// <summary>.An item the Kiosk sells.</summary>

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } 

    public decimal Price { get; set; }

    public int StockQty { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    ///<summary>The foreign key column.</summary>
    public int CategoryId { get; set; }

    ///<summary>Navigation to the parent, so code can write product. Category.Name.</summary>
    public Category Category { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc {get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

}
