using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Labor;
using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.API.Controllers;

[Authorize]
public class LaborController : BaseApiController
{
    private readonly ILaborService _laborService;

    public LaborController(ILaborService laborService)
    {
        _laborService = laborService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? farmId, [FromQuery] PaymentStatus? paymentStatus)
    {
        var labors = await _laborService.GetAllForUserAsync(CurrentUserId, farmId, paymentStatus);
        return Ok(ApiResponse<List<LaborDto>>.Ok(labors));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var labor = await _laborService.GetByIdAsync(CurrentUserId, id);
        return labor == null
            ? NotFound(ApiResponse<LaborDto>.Fail("Labor record not found."))
            : Ok(ApiResponse<LaborDto>.Ok(labor));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLaborDto dto)
    {
        var labor = await _laborService.CreateAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = labor.Id }, ApiResponse<LaborDto>.Ok(labor, "Labor entry recorded."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateLaborDto dto)
    {
        await _laborService.UpdateAsync(CurrentUserId, id, dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Labor entry updated."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _laborService.DeleteAsync(CurrentUserId, id);
        return Ok(ApiResponse<object>.Ok(new { }, "Labor entry deleted."));
    }
}
