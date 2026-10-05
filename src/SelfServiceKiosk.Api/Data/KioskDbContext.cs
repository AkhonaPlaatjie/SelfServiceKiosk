using Microsoft.EntityFrameworkCore;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Data;

/// <summary>
/// The EF Core unit of work. Add one DbSet per entity; put column rules (lengths, indexes,
/// relationships) in a class under Data/Configurations rather than in this file.
/// </summary>
public class KioskDbContext : DbContext
{
    public KioskDbContext(DbContextOptions<KioskDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Picks up every IEntityTypeConfiguration<T> in this project automatically.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KioskDbContext).Assembly);
    }
}
