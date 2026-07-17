using AgriLedger.Domain.Common;

namespace AgriLedger.Domain.Entities;

public class Farm : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public string Name { get; set; } = string.Empty;
    public double? AreaInAcres { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }

    public ICollection<Crop> Crops { get; set; } = new List<Crop>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
}
