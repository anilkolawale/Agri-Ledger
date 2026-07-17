using AgriLedger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgriLedger.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<Crop> Crops => Set<Crop>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Income> Incomes => Set<Income>();
    public DbSet<Labor> Labors => Set<Labor>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<MandiPrice> MandiPrices => Set<MandiPrice>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global soft-delete query filters.
        builder.Entity<Farm>().HasQueryFilter(f => !f.IsDeleted);
        builder.Entity<Crop>().HasQueryFilter(c => !c.IsDeleted);
        builder.Entity<Expense>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Income>().HasQueryFilter(i => !i.IsDeleted);
        builder.Entity<Labor>().HasQueryFilter(l => !l.IsDeleted);
        builder.Entity<InventoryItem>().HasQueryFilter(i => !i.IsDeleted);

        SeedData.Seed(builder);
    }
}
