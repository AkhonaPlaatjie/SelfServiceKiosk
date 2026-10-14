using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(account => account.Id);

        builder.Property(account => account.Balance)
            .HasPrecision(10, 2);

        // One-to-one: WithOne makes EF Core add a unique index on UserId, so a user can have only one account.
        builder.HasOne(account => account.User)
            .WithOne()
            .HasForeignKey<Account>(account => account.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
