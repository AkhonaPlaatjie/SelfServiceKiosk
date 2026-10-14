using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction>builder)
    {
                builder.ToTable("Transactions");

        builder.HasKey(transaction => transaction.Id);

        builder.Property(transaction => transaction.Type)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(transaction => transaction.Amount)
            .HasPrecision(10, 2);

        builder.Property(transaction => transaction.Reference)
            .HasMaxLength(100);

        builder.HasOne(transaction => transaction.Account)
            .WithMany(account => account.Transactions)
            .HasForeignKey(transaction => transaction.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
