using AgriLedger.Application.DTOs.Reports;
using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Enums;

namespace AgriLedger.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _uow;

    public ReportService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<ExpenseReportRowDto>> GetExpenseReportAsync(int userId, ReportFilterDto filter)
    {
        var query = FilterExpenses(userId, filter);
        return query
            .OrderByDescending(e => e.ExpenseDate)
            .Select(e => new ExpenseReportRowDto
            {
                ExpenseDate = e.ExpenseDate,
                FarmName = e.Farm!.Name,
                CropName = e.Crop != null ? e.Crop.Name : null,
                CategoryName = e.ExpenseCategory!.Name,
                Amount = e.Amount,
                PaymentMethod = e.PaymentMethod.ToString(),
                Description = e.Description
            }).ToList();
    }

    public async Task<List<IncomeReportRowDto>> GetIncomeReportAsync(int userId, ReportFilterDto filter)
    {
        var query = FilterIncomes(userId, filter);
        return query
            .OrderByDescending(i => i.SaleDate)
            .Select(i => new IncomeReportRowDto
            {
                SaleDate = i.SaleDate,
                FarmName = i.Farm!.Name,
                CropName = i.Crop != null ? i.Crop.Name : null,
                BuyerName = i.BuyerName,
                TotalAmount = i.TotalAmount,
                PaymentStatus = i.PaymentStatus.ToString()
            }).ToList();
    }

    public async Task<List<CropProfitReportRowDto>> GetCropProfitReportAsync(int userId, ReportFilterDto filter)
    {
        var expenses = FilterExpenses(userId, filter).Where(e => e.CropId != null).ToList();
        var incomes = FilterIncomes(userId, filter).Where(i => i.CropId != null).ToList();

        var cropIds = expenses.Select(e => e.CropId!.Value).Union(incomes.Select(i => i.CropId!.Value)).Distinct();

        var rows = new List<CropProfitReportRowDto>();
        foreach (var cropId in cropIds)
        {
            var crop = _uow.Crops.Query(c => c.Farm).FirstOrDefault(c => c.Id == cropId);
            if (crop == null) continue;
            var totalExpense = expenses.Where(e => e.CropId == cropId).Sum(e => e.Amount);
            var totalIncome = incomes.Where(i => i.CropId == cropId).Sum(i => i.TotalAmount);
            rows.Add(new CropProfitReportRowDto
            {
                FarmName = crop.Farm!.Name,
                CropName = crop.Name,
                TotalExpense = totalExpense,
                TotalIncome = totalIncome,
                Profit = totalIncome - totalExpense
            });
        }
        return rows.OrderByDescending(r => r.Profit).ToList();
    }

    public async Task<List<CategoryExpenseReportRowDto>> GetCategoryExpenseReportAsync(int userId, ReportFilterDto filter)
    {
        return FilterExpenses(userId, filter)
            .GroupBy(e => e.ExpenseCategory!.Name)
            .Select(g => new CategoryExpenseReportRowDto
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(e => e.Amount),
                TransactionCount = g.Count()
            })
            .OrderByDescending(r => r.TotalAmount)
            .ToList();
    }

    public async Task<List<FarmExpenseReportRowDto>> GetFarmExpenseReportAsync(int userId, ReportFilterDto filter)
    {
        return FilterExpenses(userId, filter)
            .GroupBy(e => e.Farm!.Name)
            .Select(g => new FarmExpenseReportRowDto { FarmName = g.Key, TotalAmount = g.Sum(e => e.Amount) })
            .OrderByDescending(r => r.TotalAmount)
            .ToList();
    }

    public async Task<List<FarmIncomeReportRowDto>> GetFarmIncomeReportAsync(int userId, ReportFilterDto filter)
    {
        return FilterIncomes(userId, filter)
            .GroupBy(i => i.Farm!.Name)
            .Select(g => new FarmIncomeReportRowDto { FarmName = g.Key, TotalAmount = g.Sum(i => i.TotalAmount) })
            .OrderByDescending(r => r.TotalAmount)
            .ToList();
    }

    public async Task<List<LaborPaymentReportRowDto>> GetLaborPaymentReportAsync(int userId, ReportFilterDto filter)
    {
        var query = _uow.Labors.Query().Where(l => l.UserId == userId && !l.IsDeleted);
        if (filter.FarmId.HasValue) query = query.Where(l => l.FarmId == filter.FarmId.Value);
        if (filter.From.HasValue) query = query.Where(l => l.WorkDate >= filter.From.Value);
        if (filter.To.HasValue) query = query.Where(l => l.WorkDate <= filter.To.Value);

        return query.OrderByDescending(l => l.WorkDate)
            .Select(l => new LaborPaymentReportRowDto
            {
                FarmName = l.Farm!.Name,
                WorkerName = l.WorkerName,
                WorkDate = l.WorkDate,
                Task = l.Task,
                DailyWage = l.DailyWage,
                PaymentStatus = l.PaymentStatus.ToString()
            }).ToList();
    }

    public async Task<List<InventoryReportRowDto>> GetInventoryReportAsync(int userId)
    {
        return _uow.InventoryItems.Query()
            .Where(i => i.UserId == userId && !i.IsDeleted)
            .Select(i => new InventoryReportRowDto
            {
                Name = i.Name,
                Type = i.Type.ToString(),
                QuantityAvailable = i.QuantityAvailable,
                Unit = i.Unit,
                IsLowStock = i.QuantityAvailable <= i.ReorderThreshold
            })
            .OrderBy(r => r.Name)
            .ToList();
    }

    public async Task<ProfitAndLossReportDto> GetProfitAndLossReportAsync(int userId, ReportFilterDto filter)
    {
        var totalExpense = FilterExpenses(userId, filter).Sum(e => (decimal?)e.Amount) ?? 0;
        var totalIncome = FilterIncomes(userId, filter).Sum(i => (decimal?)i.TotalAmount) ?? 0;

        return new ProfitAndLossReportDto
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            NetProfit = totalIncome - totalExpense,
            ExpenseBreakdown = await GetCategoryExpenseReportAsync(userId, filter),
            IncomeByFarm = await GetFarmIncomeReportAsync(userId, filter)
        };
    }

    private IQueryable<Domain.Entities.Expense> FilterExpenses(int userId, ReportFilterDto filter)
    {
        var query = _uow.Expenses.Query().Where(e => e.UserId == userId && !e.IsDeleted);
        if (filter.FarmId.HasValue) query = query.Where(e => e.FarmId == filter.FarmId.Value);
        if (filter.From.HasValue) query = query.Where(e => e.ExpenseDate >= filter.From.Value);
        if (filter.To.HasValue) query = query.Where(e => e.ExpenseDate <= filter.To.Value);
        return query;
    }

    private IQueryable<Domain.Entities.Income> FilterIncomes(int userId, ReportFilterDto filter)
    {
        var query = _uow.Incomes.Query().Where(i => i.UserId == userId && !i.IsDeleted);
        if (filter.FarmId.HasValue) query = query.Where(i => i.FarmId == filter.FarmId.Value);
        if (filter.From.HasValue) query = query.Where(i => i.SaleDate >= filter.From.Value);
        if (filter.To.HasValue) query = query.Where(i => i.SaleDate <= filter.To.Value);
        return query;
    }
}
