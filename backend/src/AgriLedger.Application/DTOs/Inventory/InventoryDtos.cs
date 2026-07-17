using AgriLedger.Domain.Enums;

namespace AgriLedger.Application.DTOs.Inventory;

public class InventoryItemDto
{
    public int Id { get; set; }
    public int? FarmId { get; set; }
    public string? FarmName { get; set; }
    public string Name { get; set; } = string.Empty;
    public InventoryItemType Type { get; set; }
    public decimal QuantityAvailable { get; set; }
    public string Unit { get; set; } = "kg";
    public decimal ReorderThreshold { get; set; }
    public bool IsLowStock { get; set; }
    public string? Notes { get; set; }
}

public class CreateInventoryItemDto
{
    public int? FarmId { get; set; }
    public string Name { get; set; } = string.Empty;
    public InventoryItemType Type { get; set; }
    public decimal QuantityAvailable { get; set; }
    public string Unit { get; set; } = "kg";
    public decimal ReorderThreshold { get; set; }
    public string? Notes { get; set; }
}

public class UpdateInventoryItemDto : CreateInventoryItemDto { }

public class AdjustInventoryDto
{
    // Positive = restock, negative = usage. Keeps "remaining quantity after usage" simple for farmers.
    public decimal QuantityDelta { get; set; }
}
