using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Expense;
using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Entities;
using AutoMapper;

namespace AgriLedger.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ExpenseService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PagedResult<ExpenseDto>> GetPagedAsync(
        int userId, PagedRequest request, int? farmId, int? cropId, DateTime? from, DateTime? to)
    {
        var query = _uow.Expenses.Query(e => e.Farm, e => e.Crop, e => e.ExpenseCategory)
            .Where(e => e.UserId == userId && !e.IsDeleted);

        if (farmId.HasValue) query = query.Where(e => e.FarmId == farmId.Value);
        if (cropId.HasValue) query = query.Where(e => e.CropId == cropId.Value);
        if (from.HasValue) query = query.Where(e => e.ExpenseDate >= from.Value);
        if (to.HasValue) query = query.Where(e => e.ExpenseDate <= to.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(e =>
                (e.Description != null && e.Description.ToLower().Contains(term)) ||
                e.Farm!.Name.ToLower().Contains(term) ||
                (e.Crop != null && e.Crop.Name.ToLower().Contains(term)) ||
                e.ExpenseCategory!.Name.ToLower().Contains(term));
        }

        query = request.SortBy?.ToLower() switch
        {
            "amount" => request.SortDescending ? query.OrderByDescending(e => e.Amount) : query.OrderBy(e => e.Amount),
            _ => request.SortDescending ? query.OrderByDescending(e => e.ExpenseDate) : query.OrderBy(e => e.ExpenseDate)
        };

        var totalCount = query.Count();
        var items = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<ExpenseDto>
        {
            Items = _mapper.Map<List<ExpenseDto>>(items),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<ExpenseDto?> GetByIdAsync(int userId, int expenseId)
    {
        var expense = GetOwnedExpense(userId, expenseId);
        return expense == null ? null : _mapper.Map<ExpenseDto>(expense);
    }

    public async Task<ExpenseDto> CreateAsync(int userId, CreateExpenseDto dto)
    {
        await EnsureFarmAndCropOwnedAsync(userId, dto.FarmId, dto.CropId);
        var expense = _mapper.Map<Expense>(dto);
        expense.UserId = userId;
        await _uow.Expenses.AddAsync(expense);
        await _uow.SaveChangesAsync();
        return _mapper.Map<ExpenseDto>(expense);
    }

    public async Task UpdateAsync(int userId, int expenseId, UpdateExpenseDto dto)
    {
        var expense = GetOwnedExpense(userId, expenseId)
            ?? throw new KeyNotFoundException("Expense not found.");

        await EnsureFarmAndCropOwnedAsync(userId, dto.FarmId, dto.CropId);

        expense.FarmId = dto.FarmId;
        expense.CropId = dto.CropId;
        expense.ExpenseCategoryId = dto.ExpenseCategoryId;
        expense.Amount = dto.Amount;
        expense.ExpenseDate = dto.ExpenseDate;
        expense.ExpenseTime = string.IsNullOrWhiteSpace(dto.ExpenseTime)
            ? null
            : TimeSpan.TryParse(dto.ExpenseTime, out var parsedTime) ? parsedTime : null;
        expense.PaymentMethod = dto.PaymentMethod;
        expense.Description = dto.Description;
        expense.UpdatedAt = DateTime.UtcNow;

        _uow.Expenses.Update(expense);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId, int expenseId)
    {
        var expense = GetOwnedExpense(userId, expenseId)
            ?? throw new KeyNotFoundException("Expense not found.");

        expense.IsDeleted = true;
        _uow.Expenses.Update(expense);
        await _uow.SaveChangesAsync();
    }

    public async Task<List<ExpenseCategoryDto>> GetCategoriesAsync(int userId)
    {
        var categories = _uow.ExpenseCategories.Query()
            .Where(c => c.IsSystemDefault || c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToList();
        return _mapper.Map<List<ExpenseCategoryDto>>(categories);
    }

    public async Task<ExpenseCategoryDto> CreateCategoryAsync(int userId, string name)
    {
        var category = new ExpenseCategory { Name = name, UserId = userId, IsSystemDefault = false };
        await _uow.ExpenseCategories.AddAsync(category);
        await _uow.SaveChangesAsync();
        return _mapper.Map<ExpenseCategoryDto>(category);
    }

    private Expense? GetOwnedExpense(int userId, int expenseId)
    {
        var expense = _uow.Expenses.Query(e => e.Farm, e => e.Crop, e => e.ExpenseCategory)
            .FirstOrDefault(e => e.Id == expenseId && e.UserId == userId);
        return expense == null || expense.IsDeleted ? null : expense;
    }

    private async Task EnsureFarmAndCropOwnedAsync(int userId, int farmId, int? cropId)
    {
        var farm = await _uow.Farms.GetByIdAsync(farmId);
        if (farm == null || farm.UserId != userId || farm.IsDeleted)
            throw new UnauthorizedAccessException("Farm not found or not owned by the current user.");

        if (cropId.HasValue)
        {
            var crop = _uow.Crops.Query(c => c.Farm).FirstOrDefault(c => c.Id == cropId.Value);
            if (crop == null || crop.IsDeleted || crop.Farm!.UserId != userId)
                throw new UnauthorizedAccessException("Crop not found or not owned by the current user.");
        }
    }
}
