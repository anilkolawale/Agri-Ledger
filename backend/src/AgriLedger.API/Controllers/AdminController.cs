using System;
using System.Linq;
using System.Threading.Tasks;
using AgriLedger.Application.DTOs.Common;
using AgriLedger.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriLedger.API.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : BaseApiController
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var totalUsers = await _db.Users.CountAsync(u => !u.IsDeleted);
        var totalFarms = await _db.Farms.CountAsync(f => !f.IsDeleted);
        var totalCrops = await _db.Crops.CountAsync(c => !c.IsDeleted);
        var totalExpenses = await _db.Expenses.Where(e => !e.IsDeleted).SumAsync(e => e.Amount);
        var totalIncomes = await _db.Incomes.Where(i => !i.IsDeleted).SumAsync(i => i.TotalAmount);

        var stats = new
        {
            TotalUsers = totalUsers,
            TotalFarms = totalFarms,
            TotalCrops = totalCrops,
            TotalExpenses = totalExpenses,
            TotalIncomes = totalIncomes
        };

        return Ok(ApiResponse<object>.Ok(stats));
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Users
            .Where(u => !u.IsDeleted)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.PhoneNumber,
                u.Role,
                u.CreatedAt,
                FarmsCount = _db.Farms.Count(f => f.UserId == u.Id && !f.IsDeleted)
            })
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return Ok(ApiResponse<object>.Ok(users));
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Role))
        {
            return BadRequest(ApiResponse<object>.Fail("Role cannot be empty"));
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        if (user == null) return NotFound(ApiResponse<object>.Fail("User not found"));

        if (id == CurrentUserId) return BadRequest(ApiResponse<object>.Fail("You cannot change your own role"));

        user.Role = request.Role;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { user.Id, user.Role }));
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        if (user == null) return NotFound(ApiResponse<object>.Fail("User not found"));

        if (id == CurrentUserId) return BadRequest(ApiResponse<object>.Fail("You cannot delete yourself"));

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok("User deleted successfully"));
    }
}

public class UpdateRoleRequest
{
    public string Role { get; set; } = string.Empty;
}
