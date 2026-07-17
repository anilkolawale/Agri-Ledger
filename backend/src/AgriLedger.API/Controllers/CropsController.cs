using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Crop;
using AgriLedger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.API.Controllers;

[Authorize]
public class CropsController : BaseApiController
{
    private readonly ICropService _cropService;

    public CropsController(ICropService cropService)
    {
        _cropService = cropService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? farmId)
    {
        var crops = await _cropService.GetAllForUserAsync(CurrentUserId, farmId);
        return Ok(ApiResponse<List<CropDto>>.Ok(crops));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var crop = await _cropService.GetByIdAsync(CurrentUserId, id);
        return crop == null
            ? NotFound(ApiResponse<CropDto>.Fail("Crop not found."))
            : Ok(ApiResponse<CropDto>.Ok(crop));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCropDto dto)
    {
        var crop = await _cropService.CreateAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = crop.Id }, ApiResponse<CropDto>.Ok(crop, "Crop created."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCropDto dto)
    {
        await _cropService.UpdateAsync(CurrentUserId, id, dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Crop updated."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _cropService.DeleteAsync(CurrentUserId, id);
        return Ok(ApiResponse<object>.Ok(new { }, "Crop deleted."));
    }
}
