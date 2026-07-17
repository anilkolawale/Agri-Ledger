using AgriLedger.Domain.Entities;
using AgriLedger.Domain.Enums;
using AgriLedger.Infrastructure.Identity;
using AgriLedger.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AgriLedger.Application.Interfaces;

namespace AgriLedger.Infrastructure.SampleData;

// Optional runtime demo-data seeder (separate from the always-on expense
// category seed in SeedData.cs). Not wired into Program.cs by default —
// call DemoDataSeeder.SeedAsync(app.Services) once after `dotnet run` if you
// want a ready-to-explore demo account instead of an empty database.
//
// Demo login after seeding: demo@agriledger.test / Demo@12345
public static class DemoDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        if (await db.Users.AnyAsync(u => u.Email == "demo@agriledger.test"))
            return; // already seeded

        var user = new User
        {
            FullName = "Ramesh Patil",
            Email = "demo@agriledger.test",
            PhoneNumber = "9876543210",
            PasswordHash = hasher.Hash("Demo@12345"),
            PreferredLanguage = "en",
            Role = "Farmer"
        };
        db.Users.Add(user);

        var adminUser = new User
        {
            FullName = "System Administrator",
            Email = "admin@agriledger.test",
            PhoneNumber = "9999999999",
            PasswordHash = hasher.Hash("Admin@12345"),
            PreferredLanguage = "en",
            Role = "Admin"
        };
        db.Users.Add(adminUser);

        await db.SaveChangesAsync();


        var grapesFarm = new Farm { UserId = user.Id, Name = "Krishna Grapes Farm", AreaInAcres = 5.0, Location = "Nashik, Maharashtra" };
        var pomegranateOrchard = new Farm { UserId = user.Id, Name = "Sai Pomegranate Farm", AreaInAcres = 3.5, Location = "Pune, Maharashtra" };
        db.Farms.AddRange(grapesFarm, pomegranateOrchard);
        await db.SaveChangesAsync();

        var grapes = new Crop
        {
            FarmId = grapesFarm.Id,
            Name = "Grapes",
            Variety = "Thompson Seedless",
            PlantDate = DateTime.UtcNow.AddDays(-120),
            ExpectedHarvestDate = DateTime.UtcNow.AddDays(30),
            Status = CropStatus.Growing
        };
        var pomegranate = new Crop
        {
            FarmId = pomegranateOrchard.Id,
            Name = "Pomegranate",
            Variety = "Bhagwa",
            PlantDate = DateTime.UtcNow.AddYears(-2),
            ExpectedHarvestDate = DateTime.UtcNow.AddDays(60),
            Status = CropStatus.Growing
        };
        var banana = new Crop
        {
            FarmId = grapesFarm.Id,
            Name = "Banana",
            Variety = "Grand Naine",
            PlantDate = DateTime.UtcNow.AddDays(-180),
            ExpectedHarvestDate = DateTime.UtcNow.AddDays(90),
            Status = CropStatus.Growing
        };
        var sugarcane = new Crop
        {
            FarmId = grapesFarm.Id,
            Name = "Sugarcane",
            Variety = "Co 86032",
            PlantDate = DateTime.UtcNow.AddDays(-240),
            ExpectedHarvestDate = DateTime.UtcNow.AddDays(120),
            Status = CropStatus.Growing
        };

        db.Crops.AddRange(grapes, pomegranate, banana, sugarcane);
        await db.SaveChangesAsync();

        var categories = await db.ExpenseCategories.Where(c => c.IsSystemDefault).ToListAsync();
        int CatId(string name) => categories.First(c => c.Name == name).Id;

        db.Expenses.AddRange(
            new Expense { UserId = user.Id, FarmId = grapesFarm.Id, CropId = grapes.Id, ExpenseCategoryId = CatId("Seeds"), Amount = 15000, ExpenseDate = DateTime.UtcNow.AddDays(-118), PaymentMethod = PaymentMethod.Cash, Description = "Grapes rootstock cuttings" },
            new Expense { UserId = user.Id, FarmId = pomegranateOrchard.Id, CropId = pomegranate.Id, ExpenseCategoryId = CatId("Fertilizer"), Amount = 8500, ExpenseDate = DateTime.UtcNow.AddDays(-40), PaymentMethod = PaymentMethod.UPI, Description = "Organic manure & micronutrients" },
            new Expense { UserId = user.Id, FarmId = grapesFarm.Id, CropId = banana.Id, ExpenseCategoryId = CatId("Pesticides"), Amount = 4200, ExpenseDate = DateTime.UtcNow.AddDays(-15), PaymentMethod = PaymentMethod.Cash, Description = "Leaf spot control spray" },
            new Expense { UserId = user.Id, FarmId = grapesFarm.Id, CropId = sugarcane.Id, ExpenseCategoryId = CatId("Irrigation"), Amount = 12000, ExpenseDate = DateTime.UtcNow.AddDays(-230), PaymentMethod = PaymentMethod.UPI, Description = "Subsurface drip line setup" }
        );

        db.Incomes.Add(new Income
        {
            UserId = user.Id, FarmId = grapesFarm.Id, CropId = grapes.Id,
            BuyerName = "Mahagrapes Exporters", Quantity = 1200, Unit = "kg", PricePerUnit = 110,
            TotalAmount = 1200 * 110, SaleDate = DateTime.UtcNow.AddDays(-3), PaymentStatus = PaymentStatus.Paid
        });

        db.Labors.Add(new Labor
        {
            UserId = user.Id, FarmId = pomegranateOrchard.Id, WorkerName = "Suresh Kale",
            WorkDate = DateTime.UtcNow.AddDays(-4), Task = "Pomegranate Pruning", DailyWage = 450, PaymentStatus = PaymentStatus.Pending
        });

        db.InventoryItems.Add(new InventoryItem
        {
            UserId = user.Id, FarmId = grapesFarm.Id, Name = "Soluble NPK 19:19:19", Type = InventoryItemType.Fertilizer,
            QuantityAvailable = 50, Unit = "kg", ReorderThreshold = 100
        });

        await db.SaveChangesAsync();

    }
}
