using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Expense;
using AgriLedger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.API.Controllers;

[Authorize]
public class ExpensesController : BaseApiController
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null, [FromQuery] bool sortDescending = true,
        [FromQuery] int? farmId = null, [FromQuery] int? cropId = null,
        [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var request = new PagedRequest
        {
            PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm,
            SortBy = sortBy, SortDescending = sortDescending
        };
        var result = await _expenseService.GetPagedAsync(CurrentUserId, request, farmId, cropId, from, to);
        return Ok(ApiResponse<PagedResult<ExpenseDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var expense = await _expenseService.GetByIdAsync(CurrentUserId, id);
        return expense == null
            ? NotFound(ApiResponse<ExpenseDto>.Fail("Expense not found."))
            : Ok(ApiResponse<ExpenseDto>.Ok(expense));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseDto dto)
    {
        var expense = await _expenseService.CreateAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, ApiResponse<ExpenseDto>.Ok(expense, "Expense recorded."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseDto dto)
    {
        await _expenseService.UpdateAsync(CurrentUserId, id, dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Expense updated."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _expenseService.DeleteAsync(CurrentUserId, id);
        return Ok(ApiResponse<object>.Ok(new { }, "Expense deleted."));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _expenseService.GetCategoriesAsync(CurrentUserId);
        return Ok(ApiResponse<List<ExpenseCategoryDto>>.Ok(categories));
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] string name)
    {
        var category = await _expenseService.CreateCategoryAsync(CurrentUserId, name);
        return Ok(ApiResponse<ExpenseCategoryDto>.Ok(category, "Custom category created."));
    }
}
