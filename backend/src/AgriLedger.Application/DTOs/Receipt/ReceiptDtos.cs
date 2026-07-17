using AgriLedger.Domain.Enums;

namespace AgriLedger.Application.DTOs.Receipt;

public class ReceiptDto
{
    public int Id { get; set; }
    public ReceiptOwnerType OwnerType { get; set; }
    public int? ExpenseId { get; set; }
    public int? IncomeId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTime UploadDate { get; set; }
}
