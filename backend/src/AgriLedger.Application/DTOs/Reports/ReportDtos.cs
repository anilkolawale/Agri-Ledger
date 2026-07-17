namespace AgriLedger.Application.DTOs.Reports;

public class ReportFilterDto
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? FarmId { get; set; }
}

public class ExpenseReportRowDto
{
    public DateTime ExpenseDate { get; set; }
    public string FarmName { get; set; } = string.Empty;
    public string? CropName { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class IncomeReportRowDto
{
    public DateTime SaleDate { get; set; }
    public string FarmName { get; set; } = string.Empty;
    public string? CropName { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
}

public class CropProfitReportRowDto
{
    public string FarmName { get; set; } = string.Empty;
    public string CropName { get; set; } = string.Empty;
    public decimal TotalExpense { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal Profit { get; set; }
}

public class CategoryExpenseReportRowDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
}

public class FarmExpenseReportRowDto
{
    public string FarmName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

public class FarmIncomeReportRowDto
{
    public string FarmName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

public class LaborPaymentReportRowDto
{
    public string FarmName { get; set; } = string.Empty;
    public string WorkerName { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public string Task { get; set; } = string.Empty;
    public decimal DailyWage { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
}

public class InventoryReportRowDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal QuantityAvailable { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool IsLowStock { get; set; }
}

public class ProfitAndLossReportDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetProfit { get; set; }
    public List<CategoryExpenseReportRowDto> ExpenseBreakdown { get; set; } = new();
    public List<FarmIncomeReportRowDto> IncomeByFarm { get; set; } = new();
}
