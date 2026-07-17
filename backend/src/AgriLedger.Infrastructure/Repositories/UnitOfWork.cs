using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Entities;
using AgriLedger.Infrastructure.Persistence;

namespace AgriLedger.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new GenericRepository<User>(context);
        Farms = new GenericRepository<Farm>(context);
        Crops = new GenericRepository<Crop>(context);
        Expenses = new GenericRepository<Expense>(context);
        Incomes = new GenericRepository<Income>(context);
        Labors = new GenericRepository<Labor>(context);
        InventoryItems = new GenericRepository<InventoryItem>(context);
        Receipts = new GenericRepository<Receipt>(context);
        ExpenseCategories = new GenericRepository<ExpenseCategory>(context);
    }

    public IGenericRepository<User> Users { get; }
    public IGenericRepository<Farm> Farms { get; }
    public IGenericRepository<Crop> Crops { get; }
    public IGenericRepository<Expense> Expenses { get; }
    public IGenericRepository<Income> Incomes { get; }
    public IGenericRepository<Labor> Labors { get; }
    public IGenericRepository<InventoryItem> InventoryItems { get; }
    public IGenericRepository<Receipt> Receipts { get; }
    public IGenericRepository<ExpenseCategory> ExpenseCategories { get; }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
