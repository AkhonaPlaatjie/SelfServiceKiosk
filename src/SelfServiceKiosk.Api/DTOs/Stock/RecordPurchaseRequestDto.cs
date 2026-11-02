using System.ComponentModel.DataAnnotations;

namespace SelfServiceKiosk.Api.DTOs.Stock;

public class RecordPurchaseRequestDto
{
    [Required(ErrorMessage = "Supplier name is required.")]
    [StringLength(100, ErrorMessage = "Supplier name must be at most 100 characters.")]
    public string SupplierName { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Product is required.")]
    public int ProductId { get; set; }

    [Range(1, 1000000, ErrorMessage = "Quantity must be between 1 and 1000000.")]
    public int Quantity { get; set; }

    [Range(0.01, 99999999.99, ErrorMessage = "Unit cost must be greater than zero.")]
    public decimal UnitCost { get; set; }

    [Required(ErrorMessage = "Purchase date is required.")]
    public DateTime? PurchaseDateUtc { get; set; }
}
