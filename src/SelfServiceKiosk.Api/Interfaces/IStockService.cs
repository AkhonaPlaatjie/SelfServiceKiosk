using SelfServiceKiosk.Api.DTOs.Common;
using SelfServiceKiosk.Api.DTOs.Products;
using SelfServiceKiosk.Api.DTOs.Stock;

namespace SelfServiceKiosk.Api.Interfaces;

public interface IStockService
{
    Task<StockPurchaseDto> RecordPurchaseAsync(
        RecordPurchaseRequestDto request,
        int adminUserId,
        CancellationToken cancellationToken = default);

    /// <summary>Records a correction of an earlier purchase. Stock can never go below zero.</summary>
    Task<StockPurchaseDto> RecordCorrectionAsync(
        int originalId,
        RecordCorrectionRequestDto request,
        int adminUserId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<StockPurchaseDto>> GetPageAsync(
        PageQuery query,
        int? productId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default);

    Task<StockPurchaseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductDto>> GetLowStockAsync(CancellationToken cancellationToken = default);
}
