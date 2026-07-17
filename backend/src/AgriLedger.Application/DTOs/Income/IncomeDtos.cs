using AgriLedger.Domain.Enums;

namespace AgriLedger.Application.DTOs.Income;

public class IncomeDto
{
    public int Id { get; set; }
    public int FarmId { get; set; }
    public string FarmName { get; set; } = string.Empty;
    public int? CropId { get; set; }
    public string? CropName { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "kg";
    public decimal PricePerUnit { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime SaleDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? Notes { get; set; }
}

public class CreateIncomeDto
{
    public int FarmId { get; set; }
    public int? CropId { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "kg";
    public decimal PricePerUnit { get; set; }
    public DateTime SaleDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? Notes { get; set; }
}

public class UpdateIncomeDto : CreateIncomeDto { }
