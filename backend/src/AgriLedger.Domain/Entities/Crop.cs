using AgriLedger.Domain.Common;
using AgriLedger.Domain.Enums;

namespace AgriLedger.Domain.Entities;

public class Crop : BaseEntity
{
    public int FarmId { get; set; }
    public Farm? Farm { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Variety { get; set; }
    public DateTime? PlantDate { get; set; }
    public DateTime? ExpectedHarvestDate { get; set; }
    public DateTime? ActualHarvestDate { get; set; }
    public CropStatus Status { get; set; } = CropStatus.Planned;
    public string? Notes { get; set; }

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
}
