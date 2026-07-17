using AgriLedger.Domain.Common;

namespace AgriLedger.Domain.Entities;

public class ExpenseCategory : BaseEntity
{
    public int? UserId { get; set; } // null = system default category, otherwise user-created custom category
    public User? User { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSystemDefault { get; set; } = false;
}
