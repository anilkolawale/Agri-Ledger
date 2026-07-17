using AgriLedger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgriLedger.Infrastructure.Persistence;

// Seeds the 12 default expense categories described in the spec so every new
// account has a ready-to-use category list (plus unlimited custom categories per user).
public static class SeedData
{
    public static void Seed(ModelBuilder builder)
    {
        var categories = new[]
        {
            "Seeds", "Fertilizer", "Pesticides", "Labor", "Fuel", "Irrigation",
            "Electricity", "Machinery", "Equipment Repair", "Packaging", "Transport", "Miscellaneous"
        };

        var id = 1;
        var seeded = categories.Select(name => new ExpenseCategory
        {
            Id = id++,
            Name = name,
            IsSystemDefault = true,
            UserId = null
        }).ToArray();

        builder.Entity<ExpenseCategory>().HasData(seeded);
    }
}
