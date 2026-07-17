using AgriLedger.Domain.Common;
using AgriLedger.Domain.Enums;

namespace AgriLedger.Domain.Entities;

public class Expense : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public int FarmId { get; set; }
    public Farm? Farm { get; set; }

    public int? CropId { get; set; }
    public Crop? Crop { get; set; }

    public int ExpenseCategoryId { get; set; }
    public ExpenseCategory? ExpenseCategory { get; set; }

    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public TimeSpan? ExpenseTime { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public string? Description { get; set; }

    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}
