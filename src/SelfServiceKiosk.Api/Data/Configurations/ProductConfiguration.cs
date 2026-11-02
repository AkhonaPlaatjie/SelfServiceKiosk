using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(product => product.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(product => product.Price)
            .HasPrecision(10, 2);

        builder.HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
