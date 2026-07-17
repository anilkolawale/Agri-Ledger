using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Income;
using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Entities;
using AutoMapper;

namespace AgriLedger.Application.Services;

public class IncomeService : IIncomeService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public IncomeService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PagedResult<IncomeDto>> GetPagedAsync(
        int userId, PagedRequest request, int? farmId, int? cropId, DateTime? from, DateTime? to)
    {
        var query = _uow.Incomes.Query(i => i.Farm, i => i.Crop).Where(i => i.UserId == userId && !i.IsDeleted);

        if (farmId.HasValue) query = query.Where(i => i.FarmId == farmId.Value);
        if (cropId.HasValue) query = query.Where(i => i.CropId == cropId.Value);
        if (from.HasValue) query = query.Where(i => i.SaleDate >= from.Value);
        if (to.HasValue) query = query.Where(i => i.SaleDate <= to.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(i =>
                i.BuyerName.ToLower().Contains(term) ||
                i.Farm!.Name.ToLower().Contains(term) ||
                (i.Crop != null && i.Crop.Name.ToLower().Contains(term)));
        }

        query = request.SortBy?.ToLower() switch
        {
            "amount" => request.SortDescending ? query.OrderByDescending(i => i.TotalAmount) : query.OrderBy(i => i.TotalAmount),
            _ => request.SortDescending ? query.OrderByDescending(i => i.SaleDate) : query.OrderBy(i => i.SaleDate)
        };

        var totalCount = query.Count();
        var items = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<IncomeDto>
        {
            Items = _mapper.Map<List<IncomeDto>>(items),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IncomeDto?> GetByIdAsync(int userId, int incomeId)
    {
        var income = GetOwnedIncome(userId, incomeId);
        return income == null ? null : _mapper.Map<IncomeDto>(income);
    }

    public async Task<IncomeDto> CreateAsync(int userId, CreateIncomeDto dto)
    {
        await EnsureFarmAndCropOwnedAsync(userId, dto.FarmId, dto.CropId);
        var income = _mapper.Map<Income>(dto);
        income.UserId = userId;
        income.TotalAmount = dto.Quantity * dto.PricePerUnit;
        await _uow.Incomes.AddAsync(income);
        await _uow.SaveChangesAsync();
        return _mapper.Map<IncomeDto>(income);
    }

    public async Task UpdateAsync(int userId, int incomeId, UpdateIncomeDto dto)
    {
        var income = GetOwnedIncome(userId, incomeId)
            ?? throw new KeyNotFoundException("Income not found.");

        await EnsureFarmAndCropOwnedAsync(userId, dto.FarmId, dto.CropId);

        income.FarmId = dto.FarmId;
        income.CropId = dto.CropId;
        income.BuyerName = dto.BuyerName;
        income.Quantity = dto.Quantity;
        income.Unit = dto.Unit;
        income.PricePerUnit = dto.PricePerUnit;
        income.TotalAmount = dto.Quantity * dto.PricePerUnit;
        income.SaleDate = dto.SaleDate;
        income.PaymentStatus = dto.PaymentStatus;
        income.Notes = dto.Notes;
        income.UpdatedAt = DateTime.UtcNow;

        _uow.Incomes.Update(income);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId, int incomeId)
    {
        var income = GetOwnedIncome(userId, incomeId)
            ?? throw new KeyNotFoundException("Income not found.");

        income.IsDeleted = true;
        _uow.Incomes.Update(income);
        await _uow.SaveChangesAsync();
    }

    private Income? GetOwnedIncome(int userId, int incomeId)
    {
        var income = _uow.Incomes.Query(i => i.Farm, i => i.Crop)
            .FirstOrDefault(i => i.Id == incomeId && i.UserId == userId);
        return income == null || income.IsDeleted ? null : income;
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

