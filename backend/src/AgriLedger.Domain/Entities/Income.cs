using AgriLedger.Domain.Common;
using AgriLedger.Domain.Enums;

namespace AgriLedger.Domain.Entities;

public class Income : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public int FarmId { get; set; }
    public Farm? Farm { get; set; }

    public int? CropId { get; set; }
    public Crop? Crop { get; set; }

    public string BuyerName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "kg"; // kg, quintal, ton, bag, etc.
    public decimal PricePerUnit { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime SaleDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? Notes { get; set; }

    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}
