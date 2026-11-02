using SelfServiceKiosk.Api.DTOs.Common;
using SelfServiceKiosk.Api.DTOs.Products;
using SelfServiceKiosk.Api.DTOs.Stock;
using SelfServiceKiosk.Api.Exceptions;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Mappers;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepository;

    public StockService(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<StockPurchaseDto> RecordPurchaseAsync(
        RecordPurchaseRequestDto request,
        int adminUserId,
        CancellationToken cancellationToken = default)
    {
        var product = await _stockRepository.GetProductAsync(
            request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new BadRequestException("The selected product does not exist.");
        }

        if (!product.IsActive)
        {
            throw new BadRequestException("Stock cannot be recorded for an inactive product.");
        }

        if (request.PurchaseDateUtc is null ||
            request.PurchaseDateUtc.Value > DateTime.UtcNow)
        {
            throw new BadRequestException("Purchase date cannot be in the future.");
        }

        if (request.Quantity <= 0)
        {
            throw new BadRequestException("Purchase quantity must be greater than zero.");
        }

        if (request.UnitCost <= 0)
        {
            throw new BadRequestException("Unit cost must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(request.SupplierName))
        {
            throw new BadRequestException("Supplier name is required.");
        }

        product.StockQty += request.Quantity;

        var purchase = new StockPurchase
        {
            SupplierName = request.SupplierName.Trim(),
            ProductId = product.Id,
            Quantity = request.Quantity,
            UnitCost = request.UnitCost,
            PurchaseDateUtc = request.PurchaseDateUtc.Value,
            CreatedByUserId = adminUserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _stockRepository.AddPurchaseAsync(
            purchase, cancellationToken);

        return purchase.ToDto();
    }

    public async Task<StockPurchaseDto> RecordCorrectionAsync(
        int originalId,
        RecordCorrectionRequestDto request,
        int adminUserId,
        CancellationToken cancellationToken = default)
    {
        var originalPurchase = await _stockRepository.GetPurchaseAsync(
            originalId, cancellationToken);

        if (originalPurchase is null)
        {
            throw new NotFoundException("The original stock purchase was not found.");
        }

        if (request.QuantityAdjustment == 0)
        {
            throw new BadRequestException("Quantity adjustment cannot be zero.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            throw new BadRequestException("A reason for the correction is required.");
        }

        var product = await _stockRepository.GetProductAsync(
            originalPurchase.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException("The product associated with this purchase was not found.");
        }

        var adjustedStock = (long)product.StockQty + request.QuantityAdjustment;

        if (adjustedStock < 0)
        {
            throw new ConflictException(
                "The correction would cause stock to fall below zero.");
        }

        if (adjustedStock > int.MaxValue)
        {
            throw new BadRequestException("The correction exceeds the supported stock quantity.");
        }

        product.StockQty = (int)adjustedStock;

        var correction = new StockPurchase
        {
            SupplierName = originalPurchase.SupplierName,
            ProductId = originalPurchase.ProductId,
            Quantity = request.QuantityAdjustment,
            UnitCost = originalPurchase.UnitCost,
            PurchaseDateUtc = DateTime.UtcNow,
            CorrectsPurchaseId = originalPurchase.Id,
            Reason = request.Reason.Trim(),
            CreatedByUserId = adminUserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _stockRepository.AddPurchaseAsync(
            correction, cancellationToken);

        return correction.ToDto();
    }

    public async Task<PagedResult<StockPurchaseDto>> GetPageAsync(
        PageQuery query,
        int? productId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _stockRepository.GetPageAsync(
            query.Page,
            query.PageSize,
            productId,
            fromUtc,
            toUtc,
            cancellationToken);

        return new PagedResult<StockPurchaseDto>
        {
            Items = items.Select(purchase => purchase.ToDto()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<StockPurchaseDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var purchase = await _stockRepository.GetPurchaseAsync(
            id, cancellationToken);

        if (purchase is null)
        {
            throw new NotFoundException("Stock purchase was not found.");
        }

        return purchase.ToDto();
    }

    public async Task<IReadOnlyList<ProductDto>> GetLowStockAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _stockRepository.GetLowStockAsync(
            cancellationToken);

        return products.Select(product => product.ToDto()).ToList();
    }
}
