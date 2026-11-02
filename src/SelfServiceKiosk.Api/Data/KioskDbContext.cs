using Microsoft.EntityFrameworkCore;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Data;

/// <summary>The EF Core (Entity Framework Core) unit of work: one DbSet per table.</summary>
public class KioskDbContext : DbContext
{
    public KioskDbContext(DbContextOptions<KioskDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<Transaction> Transactions => Set<Transaction>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<StockPurchase> StockPurchases => Set<StockPurchase>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Picks up every IEntityTypeConfiguration<T> in this project automatically.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KioskDbContext).Assembly);
    }
}
