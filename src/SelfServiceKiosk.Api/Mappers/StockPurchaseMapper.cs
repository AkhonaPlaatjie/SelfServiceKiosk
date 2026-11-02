using SelfServiceKiosk.Api.DTOs.Stock;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Mappers;

public static class StockPurchaseMapper
{
    public static StockPurchaseDto ToDto(this StockPurchase purchase)
    {
        return new StockPurchaseDto
        {
            Id = purchase.Id,
            SupplierName = purchase.SupplierName,
            ProductId = purchase.ProductId,
            Quantity = purchase.Quantity,
            UnitCost = purchase.UnitCost,
            PurchaseDateUtc = purchase.PurchaseDateUtc,
            CorrectsPurchaseId = purchase.CorrectsPurchaseId,
            Reason = purchase.Reason,
            CreatedByUserId = purchase.CreatedByUserId,
            CreatedAtUtc = purchase.CreatedAtUtc,
        };
    }
}
