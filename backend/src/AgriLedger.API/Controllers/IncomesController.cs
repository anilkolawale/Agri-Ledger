using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Income;
using AgriLedger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.API.Controllers;

[Authorize]
public class IncomesController : BaseApiController
{
    private readonly IIncomeService _incomeService;

    public IncomesController(IIncomeService incomeService)
    {
        _incomeService = incomeService;
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
        var result = await _incomeService.GetPagedAsync(CurrentUserId, request, farmId, cropId, from, to);
        return Ok(ApiResponse<PagedResult<IncomeDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var income = await _incomeService.GetByIdAsync(CurrentUserId, id);
        return income == null
            ? NotFound(ApiResponse<IncomeDto>.Fail("Income not found."))
            : Ok(ApiResponse<IncomeDto>.Ok(income));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeDto dto)
    {
        var income = await _incomeService.CreateAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = income.Id }, ApiResponse<IncomeDto>.Ok(income, "Income recorded."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateIncomeDto dto)
    {
        await _incomeService.UpdateAsync(CurrentUserId, id, dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Income updated."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _incomeService.DeleteAsync(CurrentUserId, id);
        return Ok(ApiResponse<object>.Ok(new { }, "Income deleted."));
    }
}
