using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Data.Configurations;

public class StockPurchaseConfiguration : IEntityTypeConfiguration<StockPurchase>
{
    public void Configure(EntityTypeBuilder<StockPurchase> builder)
    {
        builder.ToTable("StockPurchases");

        builder.HasKey(purchase => purchase.Id);

        builder.Property(purchase => purchase.SupplierName).IsRequired().HasMaxLength(100);
        builder.Property(purchase => purchase.UnitCost).HasPrecision(10, 2);
        builder.Property(purchase => purchase.Reason).HasMaxLength(200);

        builder.HasOne(purchase => purchase.Product)
            .WithMany()
            .HasForeignKey(purchase => purchase.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(purchase => purchase.CorrectsPurchase)
            .WithMany()
            .HasForeignKey(purchase => purchase.CorrectsPurchaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(purchase => purchase.CreatedByUser)
            .WithMany()
            .HasForeignKey(purchase => purchase.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(purchase => purchase.ProductId);
        builder.HasIndex(purchase => purchase.PurchaseDateUtc);
    }
}
