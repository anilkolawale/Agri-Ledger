using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Farm;
using AgriLedger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.API.Controllers;

[Authorize]
public class FarmsController : BaseApiController
{
    private readonly IFarmService _farmService;

    public FarmsController(IFarmService farmService)
    {
        _farmService = farmService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var farms = await _farmService.GetAllForUserAsync(CurrentUserId);
        return Ok(ApiResponse<List<FarmDto>>.Ok(farms));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var farm = await _farmService.GetByIdAsync(CurrentUserId, id);
        return farm == null
            ? NotFound(ApiResponse<FarmDto>.Fail("Farm not found."))
            : Ok(ApiResponse<FarmDto>.Ok(farm));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateFarmDto dto)
    {
        var farm = await _farmService.CreateAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = farm.Id }, ApiResponse<FarmDto>.Ok(farm, "Farm created."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateFarmDto dto)
    {
        await _farmService.UpdateAsync(CurrentUserId, id, dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Farm updated."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _farmService.DeleteAsync(CurrentUserId, id);
        return Ok(ApiResponse<object>.Ok(new { }, "Farm deleted."));
    }
}
