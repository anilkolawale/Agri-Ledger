using AgriLedger.Domain.Common;
using AgriLedger.Domain.Enums;

namespace AgriLedger.Domain.Entities;

public class Receipt : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public ReceiptOwnerType OwnerType { get; set; }
    public int? ExpenseId { get; set; }
    public Expense? Expense { get; set; }
    public int? IncomeId { get; set; }
    public Income? Income { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
}
