using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Receipt;
using AgriLedger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.API.Controllers;

[Authorize]
public class ReceiptsController : BaseApiController
{
    private readonly IReceiptService _receiptService;

    public ReceiptsController(IReceiptService receiptService)
    {
        _receiptService = receiptService;
    }

    // multipart/form-data upload — used for camera photos, bills, invoices, product photos.
    [HttpPost("expense/{expenseId:int}")]
    [RequestSizeLimit(15_000_000)]
    public async Task<IActionResult> UploadForExpense(int expenseId, IFormFile file)
    {
        var receipt = await _receiptService.UploadForExpenseAsync(CurrentUserId, expenseId, file);
        return Ok(ApiResponse<ReceiptDto>.Ok(receipt, "Receipt uploaded."));
    }

    [HttpPost("income/{incomeId:int}")]
    [RequestSizeLimit(15_000_000)]
    public async Task<IActionResult> UploadForIncome(int incomeId, IFormFile file)
    {
        var receipt = await _receiptService.UploadForIncomeAsync(CurrentUserId, incomeId, file);
        return Ok(ApiResponse<ReceiptDto>.Ok(receipt, "Receipt uploaded."));
    }

    [HttpGet("expense/{expenseId:int}")]
    public async Task<IActionResult> GetForExpense(int expenseId)
    {
        var receipts = await _receiptService.GetForExpenseAsync(CurrentUserId, expenseId);
        return Ok(ApiResponse<List<ReceiptDto>>.Ok(receipts));
    }

    [HttpGet("income/{incomeId:int}")]
    public async Task<IActionResult> GetForIncome(int incomeId)
    {
        var receipts = await _receiptService.GetForIncomeAsync(CurrentUserId, incomeId);
        return Ok(ApiResponse<List<ReceiptDto>>.Ok(receipts));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _receiptService.DeleteAsync(CurrentUserId, id);
        return Ok(ApiResponse<object>.Ok(new { }, "Receipt deleted."));
    }
}
