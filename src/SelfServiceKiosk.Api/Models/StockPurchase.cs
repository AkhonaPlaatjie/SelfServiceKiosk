namespace SelfServiceKiosk.Api.Models;

/// <summary>An audit record of stock bought, or a correction of an earlier purchase. Never edited or deleted.</summary>
public class StockPurchase
{
    public int Id { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    /// <summary>Positive for a purchase. A correction may be positive or negative.</summary>
    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public DateTime PurchaseDateUtc { get; set; }

    /// <summary>Set only on a correction: the purchase it corrects.</summary>
    public int? CorrectsPurchaseId { get; set; }

    public StockPurchase? CorrectsPurchase { get; set; }

    public string? Reason { get; set; }

    public int CreatedByUserId { get; set; }

    public User CreatedByUser { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }
}
