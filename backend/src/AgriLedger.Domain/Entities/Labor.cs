using AgriLedger.Domain.Common;
using AgriLedger.Domain.Enums;

namespace AgriLedger.Domain.Entities;

public class Labor : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }
    public int FarmId { get; set; }
    public Farm? Farm { get; set; }

    public string WorkerName { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public string Task { get; set; } = string.Empty;
    public decimal DailyWage { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? Notes { get; set; }
}
