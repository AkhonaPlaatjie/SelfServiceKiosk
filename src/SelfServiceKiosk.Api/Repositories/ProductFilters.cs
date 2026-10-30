using System.Linq.Expressions;
using SelfServiceKiosk.Api.Constants;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Repositories;

public static class ProductFilters
{
    /// <summary>Active products with stock of 5 or fewer. Exactly 5 counts as low.</summary>
    public static readonly Expression<Func<Product, bool>> LowStock =
        product => product.IsActive && product.StockQty <= ProductDefaults.LowStockThreshold;
}
