using System.Linq.Expressions;

namespace AgriLedger.Application.Interfaces;

// Generic repository abstraction (Repository Pattern) shared by every entity.
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    IQueryable<T> Query();

    // Same as Query(), but eager-loads the given navigation properties (e.g. x => x.Farm).
    // Kept on the repository (not raw EF Include calls in Application) so the Application
    // layer never has to reference EF Core directly.
    IQueryable<T> Query(params Expression<Func<T, object?>>[] includes);
}

public interface IUnitOfWork
{
    IGenericRepository<Domain.Entities.User> Users { get; }
    IGenericRepository<Domain.Entities.Farm> Farms { get; }
    IGenericRepository<Domain.Entities.Crop> Crops { get; }
    IGenericRepository<Domain.Entities.Expense> Expenses { get; }
    IGenericRepository<Domain.Entities.Income> Incomes { get; }
    IGenericRepository<Domain.Entities.Labor> Labors { get; }
    IGenericRepository<Domain.Entities.InventoryItem> InventoryItems { get; }
    IGenericRepository<Domain.Entities.Receipt> Receipts { get; }
    IGenericRepository<Domain.Entities.ExpenseCategory> ExpenseCategories { get; }
    Task<int> SaveChangesAsync();
}
