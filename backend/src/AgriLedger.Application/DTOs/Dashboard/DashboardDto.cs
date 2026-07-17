namespace AgriLedger.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int TotalFarms { get; set; }
    public decimal TodaysExpenses { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public int CropsReadyForHarvest { get; set; }
    public int PendingLaborPayments { get; set; }
    public int LowInventoryItems { get; set; }
    public List<UpcomingHarvestDto> UpcomingHarvests { get; set; } = new();
}

public class UpcomingHarvestDto
{
    public string FarmName { get; set; } = string.Empty;
    public string CropName { get; set; } = string.Empty;
    public DateTime? ExpectedHarvestDate { get; set; }
}
