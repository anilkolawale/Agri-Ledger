using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Inventory;
using AgriLedger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.API.Controllers;

[Authorize]
[Route("api/inventory")]
public class InventoryController : BaseApiController
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? farmId)
    {
        var items = await _inventoryService.GetAllForUserAsync(CurrentUserId, farmId);
        return Ok(ApiResponse<List<InventoryItemDto>>.Ok(items));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _inventoryService.GetByIdAsync(CurrentUserId, id);
        return item == null
            ? NotFound(ApiResponse<InventoryItemDto>.Fail("Inventory item not found."))
            : Ok(ApiResponse<InventoryItemDto>.Ok(item));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInventoryItemDto dto)
    {
        var item = await _inventoryService.CreateAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ApiResponse<InventoryItemDto>.Ok(item, "Inventory item created."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateInventoryItemDto dto)
    {
        await _inventoryService.UpdateAsync(CurrentUserId, id, dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Inventory item updated."));
    }

    [HttpPost("{id:int}/adjust")]
    public async Task<IActionResult> Adjust(int id, AdjustInventoryDto dto)
    {
        await _inventoryService.AdjustQuantityAsync(CurrentUserId, id, dto);
        return Ok(ApiResponse<object>.Ok(new { }, "Stock quantity updated."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _inventoryService.DeleteAsync(CurrentUserId, id);
        return Ok(ApiResponse<object>.Ok(new { }, "Inventory item deleted."));
    }
}
