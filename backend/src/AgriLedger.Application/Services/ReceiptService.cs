using AgriLedger.Application.DTOs.Receipt;
using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Entities;
using AgriLedger.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace AgriLedger.Application.Services;

public class ReceiptService : IReceiptService
{
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _fileStorage;

    public ReceiptService(IUnitOfWork uow, IFileStorageService fileStorage)
    {
        _uow = uow;
        _fileStorage = fileStorage;
    }

    public async Task<ReceiptDto> UploadForExpenseAsync(int userId, int expenseId, IFormFile file)
    {
        var expense = _uow.Expenses.Query().FirstOrDefault(e => e.Id == expenseId && e.UserId == userId)
            ?? throw new KeyNotFoundException("Expense not found.");

        var (fileName, relativePath, size) = await _fileStorage.SaveAsync(file, "expenses");
        var receipt = new Receipt
        {
            UserId = userId,
            OwnerType = ReceiptOwnerType.Expense,
            ExpenseId = expense.Id,
            FileName = fileName,
            FilePath = relativePath,
            FileSizeBytes = size
        };
        await _uow.Receipts.AddAsync(receipt);
        await _uow.SaveChangesAsync();
        return ToDto(receipt);
    }

    public async Task<ReceiptDto> UploadForIncomeAsync(int userId, int incomeId, IFormFile file)
    {
        var income = _uow.Incomes.Query().FirstOrDefault(i => i.Id == incomeId && i.UserId == userId)
            ?? throw new KeyNotFoundException("Income record not found.");

        var (fileName, relativePath, size) = await _fileStorage.SaveAsync(file, "income");
        var receipt = new Receipt
        {
            UserId = userId,
            OwnerType = ReceiptOwnerType.Income,
            IncomeId = income.Id,
            FileName = fileName,
            FilePath = relativePath,
            FileSizeBytes = size
        };
        await _uow.Receipts.AddAsync(receipt);
        await _uow.SaveChangesAsync();
        return ToDto(receipt);
    }

    public async Task<List<ReceiptDto>> GetForExpenseAsync(int userId, int expenseId)
    {
        var receipts = _uow.Receipts.Query().Where(r => r.UserId == userId && r.ExpenseId == expenseId && !r.IsDeleted).ToList();
        return receipts.Select(ToDto).ToList();
    }

    public async Task<List<ReceiptDto>> GetForIncomeAsync(int userId, int incomeId)
    {
        var receipts = _uow.Receipts.Query().Where(r => r.UserId == userId && r.IncomeId == incomeId && !r.IsDeleted).ToList();
        return receipts.Select(ToDto).ToList();
    }

    public async Task DeleteAsync(int userId, int receiptId)
    {
        var receipt = _uow.Receipts.Query().FirstOrDefault(r => r.Id == receiptId && r.UserId == userId)
            ?? throw new KeyNotFoundException("Receipt not found.");
        receipt.IsDeleted = true;
        _uow.Receipts.Update(receipt);
        await _uow.SaveChangesAsync();
        // Note: physical file is intentionally left on disk for audit purposes;
        // add a cleanup job if you need hard deletes.
    }

    private ReceiptDto ToDto(Receipt r) => new()
    {
        Id = r.Id,
        OwnerType = r.OwnerType,
        ExpenseId = r.ExpenseId,
        IncomeId = r.IncomeId,
        FileName = r.FileName,
        FileUrl = _fileStorage.GetPublicUrl(r.FilePath),
        FileSizeBytes = r.FileSizeBytes,
        UploadDate = r.UploadDate
    };
}
