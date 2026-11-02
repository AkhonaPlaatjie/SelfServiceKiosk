using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Data.Configurations;

/// <summary>Table and column rules for <see cref="User"/>.</summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.Surname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(256);

        // Login looks accounts up by email, so it is indexed (NFR-15) and must be unique.
        builder.HasIndex(user => user.Email)
            .IsUnique();

        // The admin user list searches by name, so those columns are indexed too.
        builder.HasIndex(user => user.Surname);
        builder.HasIndex(user => user.FirstName);

        builder.Property(user => user.MobileNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(user => user.PasswordHash)
            .IsRequired();

        // The enum is saved as its name ("User" / "SuperUser").
        builder.Property(user => user.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}
