using AgriLedger.Application.DTOs.Auth;
using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Crop;
using AgriLedger.Application.DTOs.Dashboard;
using AgriLedger.Application.DTOs.Expense;
using AgriLedger.Application.DTOs.Farm;
using AgriLedger.Application.DTOs.Income;

namespace AgriLedger.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    Task ForgotPasswordAsync(ForgotPasswordRequestDto dto);
    Task ResetPasswordAsync(ResetPasswordRequestDto dto);
    Task ChangePasswordAsync(int userId, ChangePasswordRequestDto dto);
    Task UpdateProfileAsync(int userId, UpdateProfileRequestDto dto);
}

public interface IFarmService
{
    Task<List<FarmDto>> GetAllForUserAsync(int userId);
    Task<FarmDto?> GetByIdAsync(int userId, int farmId);
    Task<FarmDto> CreateAsync(int userId, CreateFarmDto dto);
    Task UpdateAsync(int userId, int farmId, UpdateFarmDto dto);
    Task DeleteAsync(int userId, int farmId);
}

public interface ICropService
{
    Task<List<CropDto>> GetAllForUserAsync(int userId, int? farmId);
    Task<CropDto?> GetByIdAsync(int userId, int cropId);
    Task<CropDto> CreateAsync(int userId, CreateCropDto dto);
    Task UpdateAsync(int userId, int cropId, UpdateCropDto dto);
    Task DeleteAsync(int userId, int cropId);
}

public interface IExpenseService
{
    Task<PagedResult<ExpenseDto>> GetPagedAsync(int userId, PagedRequest request, int? farmId, int? cropId, DateTime? from, DateTime? to);
    Task<ExpenseDto?> GetByIdAsync(int userId, int expenseId);
    Task<ExpenseDto> CreateAsync(int userId, CreateExpenseDto dto);
    Task UpdateAsync(int userId, int expenseId, UpdateExpenseDto dto);
    Task DeleteAsync(int userId, int expenseId);
    Task<List<ExpenseCategoryDto>> GetCategoriesAsync(int userId);
    Task<ExpenseCategoryDto> CreateCategoryAsync(int userId, string name);
}

public interface IIncomeService
{
    Task<PagedResult<IncomeDto>> GetPagedAsync(int userId, PagedRequest request, int? farmId, int? cropId, DateTime? from, DateTime? to);
    Task<IncomeDto?> GetByIdAsync(int userId, int incomeId);
    Task<IncomeDto> CreateAsync(int userId, CreateIncomeDto dto);
    Task UpdateAsync(int userId, int incomeId, UpdateIncomeDto dto);
    Task DeleteAsync(int userId, int incomeId);
}

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(int userId);
}
