using AgriLedger.Domain.Enums;

namespace AgriLedger.Application.DTOs.Labor;

public class LaborDto
{
    public int Id { get; set; }
    public int FarmId { get; set; }
    public string FarmName { get; set; } = string.Empty;
    public string WorkerName { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public string Task { get; set; } = string.Empty;
    public decimal DailyWage { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? Notes { get; set; }
}

public class CreateLaborDto
{
    public int FarmId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public string Task { get; set; } = string.Empty;
    public decimal DailyWage { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? Notes { get; set; }
}

public class UpdateLaborDto : CreateLaborDto { }
