using AgriLedger.Domain.Enums;

namespace AgriLedger.Application.DTOs.Expense;

public class ExpenseDto
{
    public int Id { get; set; }
    public int FarmId { get; set; }
    public string FarmName { get; set; } = string.Empty;
    public int? CropId { get; set; }
    public string? CropName { get; set; }
    public int ExpenseCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    // Stored as "HH:mm" text (not TimeSpan) so it round-trips cleanly with an HTML
    // <input type="time"> on the frontend without JSON TimeSpan format mismatches.
    public string? ExpenseTime { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Description { get; set; }
}

public class CreateExpenseDto
{
    public int FarmId { get; set; }
    public int? CropId { get; set; }
    public int ExpenseCategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    // Stored as "HH:mm" text (not TimeSpan) so it round-trips cleanly with an HTML
    // <input type="time"> on the frontend without JSON TimeSpan format mismatches.
    public string? ExpenseTime { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public string? Description { get; set; }
}

public class UpdateExpenseDto : CreateExpenseDto { }

public class ExpenseCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSystemDefault { get; set; }
}
