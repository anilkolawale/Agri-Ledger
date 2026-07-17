using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Reports;
using AgriLedger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.API.Controllers;

[Authorize]
public class ReportsController : BaseApiController
{
    private readonly IReportService _reportService;
    private readonly IReportExportService _exportService;

    public ReportsController(IReportService reportService, IReportExportService exportService)
    {
        _reportService = reportService;
        _exportService = exportService;
    }

    // ---------- Report data (drives the on-screen tables: search/filter/sort/pagination happen client-side over these rows) ----------

    [HttpGet("expenses")]
    public async Task<IActionResult> Expenses([FromQuery] ReportFilterDto filter)
    {
        var rows = await _reportService.GetExpenseReportAsync(CurrentUserId, filter);
        return Ok(ApiResponse<List<ExpenseReportRowDto>>.Ok(rows));
    }

    [HttpGet("income")]
    public async Task<IActionResult> Income([FromQuery] ReportFilterDto filter)
    {
        var rows = await _reportService.GetIncomeReportAsync(CurrentUserId, filter);
        return Ok(ApiResponse<List<IncomeReportRowDto>>.Ok(rows));
    }

    [HttpGet("crop-profit")]
    public async Task<IActionResult> CropProfit([FromQuery] ReportFilterDto filter)
    {
        var rows = await _reportService.GetCropProfitReportAsync(CurrentUserId, filter);
        return Ok(ApiResponse<List<CropProfitReportRowDto>>.Ok(rows));
    }

    [HttpGet("category-expense")]
    public async Task<IActionResult> CategoryExpense([FromQuery] ReportFilterDto filter)
    {
        var rows = await _reportService.GetCategoryExpenseReportAsync(CurrentUserId, filter);
        return Ok(ApiResponse<List<CategoryExpenseReportRowDto>>.Ok(rows));
    }

    [HttpGet("farm-expense")]
    public async Task<IActionResult> FarmExpense([FromQuery] ReportFilterDto filter)
    {
        var rows = await _reportService.GetFarmExpenseReportAsync(CurrentUserId, filter);
        return Ok(ApiResponse<List<FarmExpenseReportRowDto>>.Ok(rows));
    }

    [HttpGet("farm-income")]
    public async Task<IActionResult> FarmIncome([FromQuery] ReportFilterDto filter)
    {
        var rows = await _reportService.GetFarmIncomeReportAsync(CurrentUserId, filter);
        return Ok(ApiResponse<List<FarmIncomeReportRowDto>>.Ok(rows));
    }

    [HttpGet("labor-payments")]
    public async Task<IActionResult> LaborPayments([FromQuery] ReportFilterDto filter)
    {
        var rows = await _reportService.GetLaborPaymentReportAsync(CurrentUserId, filter);
        return Ok(ApiResponse<List<LaborPaymentReportRowDto>>.Ok(rows));
    }

    [HttpGet("inventory")]
    public async Task<IActionResult> Inventory()
    {
        var rows = await _reportService.GetInventoryReportAsync(CurrentUserId);
        return Ok(ApiResponse<List<InventoryReportRowDto>>.Ok(rows));
    }

    [HttpGet("profit-loss")]
    public async Task<IActionResult> ProfitAndLoss([FromQuery] ReportFilterDto filter)
    {
        var report = await _reportService.GetProfitAndLossReportAsync(CurrentUserId, filter);
        return Ok(ApiResponse<ProfitAndLossReportDto>.Ok(report));
    }

    // ---------- Excel / PDF export ----------
    // reportType: expenses | income | crop-profit | category-expense | farm-expense | farm-income | labor-payments | inventory

    [HttpGet("export/{reportType}")]
    public async Task<IActionResult> Export(string reportType, [FromQuery] ReportFilterDto filter, [FromQuery] string format = "excel")
    {
        byte[] bytes;
        string fileName;

        switch (reportType)
        {
            case "expenses":
                var expenseRows = await _reportService.GetExpenseReportAsync(CurrentUserId, filter);
                bytes = Build(expenseRows, "Expenses", format,
                    new[] { "Date", "Farm", "Crop", "Category", "Amount", "Payment", "Description" },
                    r => new[] { r.ExpenseDate.ToShortDateString(), r.FarmName, r.CropName ?? "-", r.CategoryName, r.Amount.ToString("N2"), r.PaymentMethod, r.Description ?? "" });
                fileName = "expense-report"; break;

            case "income":
                var incomeRows = await _reportService.GetIncomeReportAsync(CurrentUserId, filter);
                bytes = Build(incomeRows, "Income", format,
                    new[] { "Date", "Farm", "Crop", "Buyer", "Total", "Status" },
                    r => new[] { r.SaleDate.ToShortDateString(), r.FarmName, r.CropName ?? "-", r.BuyerName, r.TotalAmount.ToString("N2"), r.PaymentStatus });
                fileName = "income-report"; break;

            case "crop-profit":
                var cropRows = await _reportService.GetCropProfitReportAsync(CurrentUserId, filter);
                bytes = Build(cropRows, "Crop Profit", format,
                    new[] { "Farm", "Crop", "Expense", "Income", "Profit" },
                    r => new[] { r.FarmName, r.CropName, r.TotalExpense.ToString("N2"), r.TotalIncome.ToString("N2"), r.Profit.ToString("N2") });
                fileName = "crop-profit-report"; break;

            case "category-expense":
                var catRows = await _reportService.GetCategoryExpenseReportAsync(CurrentUserId, filter);
                bytes = Build(catRows, "Category Expenses", format,
                    new[] { "Category", "Total", "Transactions" },
                    r => new[] { r.CategoryName, r.TotalAmount.ToString("N2"), r.TransactionCount.ToString() });
                fileName = "category-expense-report"; break;

            case "farm-expense":
                var farmExpRows = await _reportService.GetFarmExpenseReportAsync(CurrentUserId, filter);
                bytes = Build(farmExpRows, "Farm Expenses", format,
                    new[] { "Farm", "Total" }, r => new[] { r.FarmName, r.TotalAmount.ToString("N2") });
                fileName = "farm-expense-report"; break;

            case "farm-income":
                var farmIncRows = await _reportService.GetFarmIncomeReportAsync(CurrentUserId, filter);
                bytes = Build(farmIncRows, "Farm Income", format,
                    new[] { "Farm", "Total" }, r => new[] { r.FarmName, r.TotalAmount.ToString("N2") });
                fileName = "farm-income-report"; break;

            case "labor-payments":
                var laborRows = await _reportService.GetLaborPaymentReportAsync(CurrentUserId, filter);
                bytes = Build(laborRows, "Labor Payments", format,
                    new[] { "Farm", "Worker", "Date", "Task", "Wage", "Status" },
                    r => new[] { r.FarmName, r.WorkerName, r.WorkDate.ToShortDateString(), r.Task, r.DailyWage.ToString("N2"), r.PaymentStatus });
                fileName = "labor-payment-report"; break;

            case "inventory":
                var invRows = await _reportService.GetInventoryReportAsync(CurrentUserId);
                bytes = Build(invRows, "Inventory", format,
                    new[] { "Item", "Type", "Quantity", "Unit", "Low Stock" },
                    r => new[] { r.Name, r.Type, r.QuantityAvailable.ToString("N2"), r.Unit, r.IsLowStock ? "Yes" : "No" });
                fileName = "inventory-report"; break;

            default:
                return BadRequest(ApiResponse<object>.Fail("Unknown report type."));
        }

        var contentType = format == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var extension = format == "pdf" ? "pdf" : "xlsx";
        return File(bytes, contentType, $"{fileName}.{extension}");
    }

    private byte[] Build<T>(List<T> rows, string title, string format, string[] headers, Func<T, IEnumerable<string>> rowSelector) =>
        format == "pdf"
            ? _exportService.ExportToPdf(rows, title, headers, rowSelector)
            : _exportService.ExportToExcel(rows, title);
}
