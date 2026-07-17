using AgriLedger.Application.DTOs.Dashboard;
using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Enums;

namespace AgriLedger.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;

    public DashboardService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(int userId)
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var farms = _uow.Farms.Query().Where(f => f.UserId == userId && !f.IsDeleted);
        var expenses = _uow.Expenses.Query().Where(e => e.UserId == userId && !e.IsDeleted);
        var incomes = _uow.Incomes.Query().Where(i => i.UserId == userId && !i.IsDeleted);
        var crops = _uow.Crops.Query().Where(c => !c.IsDeleted && c.Farm!.UserId == userId);
        var labors = _uow.Labors.Query().Where(l => l.UserId == userId && !l.IsDeleted);
        var inventory = _uow.InventoryItems.Query().Where(inv => inv.UserId == userId && !inv.IsDeleted);

        var totalExpenses = expenses.Sum(e => (decimal?)e.Amount) ?? 0;
        var totalIncome = incomes.Sum(i => (decimal?)i.TotalAmount) ?? 0;

        var upcomingHarvests = crops
            .Where(c => c.Status != CropStatus.Harvested && c.ExpectedHarvestDate != null && c.ExpectedHarvestDate >= today)
            .OrderBy(c => c.ExpectedHarvestDate)
            .Take(5)
            .Select(c => new UpcomingHarvestDto
            {
                FarmName = c.Farm!.Name,
                CropName = c.Name,
                ExpectedHarvestDate = c.ExpectedHarvestDate
            })
            .ToList();

        return new DashboardSummaryDto
        {
            TotalFarms = farms.Count(),
            TodaysExpenses = expenses.Where(e => e.ExpenseDate.Date == today).Sum(e => (decimal?)e.Amount) ?? 0,
            MonthlyExpenses = expenses.Where(e => e.ExpenseDate >= monthStart).Sum(e => (decimal?)e.Amount) ?? 0,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            NetProfit = totalIncome - totalExpenses,
            CropsReadyForHarvest = crops.Count(c => c.Status == CropStatus.ReadyForHarvest),
            PendingLaborPayments = labors.Count(l => l.PaymentStatus != PaymentStatus.Paid),
            LowInventoryItems = inventory.Count(i => i.QuantityAvailable <= i.ReorderThreshold),
            UpcomingHarvests = upcomingHarvests
        };
    }
}
