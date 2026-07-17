using AgriLedger.Domain.Common;
using AgriLedger.Domain.Enums;

namespace AgriLedger.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }
    public int? FarmId { get; set; }
    public Farm? Farm { get; set; }

    public string Name { get; set; } = string.Empty;
    public InventoryItemType Type { get; set; }
    public decimal QuantityAvailable { get; set; }
    public string Unit { get; set; } = "kg";
    public decimal ReorderThreshold { get; set; }
    public string? Notes { get; set; }
}
