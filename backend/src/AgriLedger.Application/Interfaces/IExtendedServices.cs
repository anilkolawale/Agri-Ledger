using AgriLedger.Application.DTOs.Inventory;
using AgriLedger.Application.DTOs.Labor;
using AgriLedger.Application.DTOs.Receipt;
using AgriLedger.Application.DTOs.Reports;
using AgriLedger.Application.DTOs.Search;
using AgriLedger.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace AgriLedger.Application.Interfaces;

public interface ILaborService
{
    Task<List<LaborDto>> GetAllForUserAsync(int userId, int? farmId, PaymentStatus? paymentStatus);
    Task<LaborDto?> GetByIdAsync(int userId, int laborId);
    Task<LaborDto> CreateAsync(int userId, CreateLaborDto dto);
    Task UpdateAsync(int userId, int laborId, UpdateLaborDto dto);
    Task DeleteAsync(int userId, int laborId);
}

public interface IInventoryService
{
    Task<List<InventoryItemDto>> GetAllForUserAsync(int userId, int? farmId);
    Task<InventoryItemDto?> GetByIdAsync(int userId, int itemId);
    Task<InventoryItemDto> CreateAsync(int userId, CreateInventoryItemDto dto);
    Task UpdateAsync(int userId, int itemId, UpdateInventoryItemDto dto);
    Task AdjustQuantityAsync(int userId, int itemId, AdjustInventoryDto dto);
    Task DeleteAsync(int userId, int itemId);
}

public interface IFileStorageService
{
    // Saves the uploaded file to disk (or a cloud provider in production) and returns
    // (storedFileName, relativePath, sizeInBytes). Validates extension + size first.
    Task<(string fileName, string relativePath, long sizeBytes)> SaveAsync(IFormFile file, string subFolder);
    string GetPublicUrl(string relativePath);
}

public interface IReceiptService
{
    Task<ReceiptDto> UploadForExpenseAsync(int userId, int expenseId, IFormFile file);
    Task<ReceiptDto> UploadForIncomeAsync(int userId, int incomeId, IFormFile file);
    Task<List<ReceiptDto>> GetForExpenseAsync(int userId, int expenseId);
    Task<List<ReceiptDto>> GetForIncomeAsync(int userId, int incomeId);
    Task DeleteAsync(int userId, int receiptId);
}

public interface IReportService
{
    Task<List<ExpenseReportRowDto>> GetExpenseReportAsync(int userId, ReportFilterDto filter);
    Task<List<IncomeReportRowDto>> GetIncomeReportAsync(int userId, ReportFilterDto filter);
    Task<List<CropProfitReportRowDto>> GetCropProfitReportAsync(int userId, ReportFilterDto filter);
    Task<List<CategoryExpenseReportRowDto>> GetCategoryExpenseReportAsync(int userId, ReportFilterDto filter);
    Task<List<FarmExpenseReportRowDto>> GetFarmExpenseReportAsync(int userId, ReportFilterDto filter);
    Task<List<FarmIncomeReportRowDto>> GetFarmIncomeReportAsync(int userId, ReportFilterDto filter);
    Task<List<LaborPaymentReportRowDto>> GetLaborPaymentReportAsync(int userId, ReportFilterDto filter);
    Task<List<InventoryReportRowDto>> GetInventoryReportAsync(int userId);
    Task<ProfitAndLossReportDto> GetProfitAndLossReportAsync(int userId, ReportFilterDto filter);
}

public interface IReportExportService
{
    byte[] ExportToExcel<T>(List<T> rows, string sheetName);
    byte[] ExportToPdf<T>(List<T> rows, string title, IEnumerable<string> columnHeaders, Func<T, IEnumerable<string>> rowSelector);
}

public interface ISearchService
{
    Task<GlobalSearchResultDto> SearchAsync(int userId, string term);
}
