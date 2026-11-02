namespace SelfServiceKiosk.Api.DTOs.Stock;

public class StockPurchaseDto
{
    public int Id { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public DateTime PurchaseDateUtc { get; set; }

    public int? CorrectsPurchaseId { get; set; }

    public string? Reason { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
