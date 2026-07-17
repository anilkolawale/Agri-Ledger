using AgriLedger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgriLedger.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.HasIndex(x => x.Email).IsUnique();
        b.Property(x => x.Email).IsRequired().HasMaxLength(256);
        b.Property(x => x.FullName).IsRequired().HasMaxLength(150);
        b.Property(x => x.PasswordHash).IsRequired();
    }
}

public class FarmConfiguration : IEntityTypeConfiguration<Farm>
{
    public void Configure(EntityTypeBuilder<Farm> b)
    {
        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.Property(x => x.AreaInAcres).HasColumnType("decimal(10,2)");
        b.HasOne(x => x.User).WithMany(x => x.Farms).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.UserId);
    }
}

public class CropConfiguration : IEntityTypeConfiguration<Crop>
{
    public void Configure(EntityTypeBuilder<Crop> b)
    {
        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.HasOne(x => x.Farm).WithMany(x => x.Crops).HasForeignKey(x => x.FarmId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.FarmId);
        b.HasIndex(x => x.Status);
    }
}

public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> b)
    {
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> b)
    {
        b.Property(x => x.Amount).HasColumnType("decimal(12,2)").IsRequired();
        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Farm).WithMany(x => x.Expenses).HasForeignKey(x => x.FarmId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Crop).WithMany(x => x.Expenses).HasForeignKey(x => x.CropId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.ExpenseCategory).WithMany().HasForeignKey(x => x.ExpenseCategoryId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.ExpenseDate);
        b.HasIndex(x => new { x.UserId, x.FarmId });
    }
}

public class IncomeConfiguration : IEntityTypeConfiguration<Income>
{
    public void Configure(EntityTypeBuilder<Income> b)
    {
        b.Property(x => x.BuyerName).IsRequired().HasMaxLength(150);
        b.Property(x => x.Quantity).HasColumnType("decimal(12,2)");
        b.Property(x => x.PricePerUnit).HasColumnType("decimal(12,2)");
        b.Property(x => x.TotalAmount).HasColumnType("decimal(12,2)");
        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Farm).WithMany(x => x.Incomes).HasForeignKey(x => x.FarmId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Crop).WithMany(x => x.Incomes).HasForeignKey(x => x.CropId).OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => x.SaleDate);
    }
}

public class LaborConfiguration : IEntityTypeConfiguration<Labor>
{
    public void Configure(EntityTypeBuilder<Labor> b)
    {
        b.Property(x => x.WorkerName).IsRequired().HasMaxLength(150);
        b.Property(x => x.DailyWage).HasColumnType("decimal(10,2)");
        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Farm).WithMany().HasForeignKey(x => x.FarmId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> b)
    {
        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.Property(x => x.QuantityAvailable).HasColumnType("decimal(12,2)");
        b.Property(x => x.ReorderThreshold).HasColumnType("decimal(12,2)");
        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Farm).WithMany().HasForeignKey(x => x.FarmId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> b)
    {
        b.Property(x => x.FileName).IsRequired().HasMaxLength(255);
        b.Property(x => x.FilePath).IsRequired().HasMaxLength(500);
        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Expense).WithMany(x => x.Receipts).HasForeignKey(x => x.ExpenseId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Income).WithMany(x => x.Receipts).HasForeignKey(x => x.IncomeId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class MandiPriceConfiguration : IEntityTypeConfiguration<MandiPrice>
{
    public void Configure(EntityTypeBuilder<MandiPrice> b)
    {
        b.Property(x => x.MarketName).IsRequired().HasMaxLength(150);
        b.Property(x => x.CropName).IsRequired().HasMaxLength(150);
        b.Property(x => x.MinPrice).HasColumnType("decimal(12,2)").IsRequired();
        b.Property(x => x.MaxPrice).HasColumnType("decimal(12,2)").IsRequired();
        b.Property(x => x.ModalPrice).HasColumnType("decimal(12,2)").IsRequired();
        b.Property(x => x.Unit).IsRequired().HasMaxLength(50);
        b.HasIndex(x => new { x.MarketName, x.CropName, x.PriceDate });
    }
}

