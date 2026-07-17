using AgriLedger.Application.DTOs.Labor;
using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Entities;
using AgriLedger.Domain.Enums;
using AutoMapper;

namespace AgriLedger.Application.Services;

public class LaborService : ILaborService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public LaborService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<LaborDto>> GetAllForUserAsync(int userId, int? farmId, PaymentStatus? paymentStatus)
    {
        var query = _uow.Labors.Query(l => l.Farm).Where(l => l.UserId == userId && !l.IsDeleted);
        if (farmId.HasValue) query = query.Where(l => l.FarmId == farmId.Value);
        if (paymentStatus.HasValue) query = query.Where(l => l.PaymentStatus == paymentStatus.Value);

        var labors = query.OrderByDescending(l => l.WorkDate).ToList();
        return _mapper.Map<List<LaborDto>>(labors);
    }

    public async Task<LaborDto?> GetByIdAsync(int userId, int laborId)
    {
        var labor = GetOwned(userId, laborId);
        return labor == null ? null : _mapper.Map<LaborDto>(labor);
    }

    public async Task<LaborDto> CreateAsync(int userId, CreateLaborDto dto)
    {
        await EnsureFarmOwnedAsync(userId, dto.FarmId);
        var labor = _mapper.Map<Domain.Entities.Labor>(dto);
        labor.UserId = userId;
        await _uow.Labors.AddAsync(labor);
        await _uow.SaveChangesAsync();
        return _mapper.Map<LaborDto>(labor);
    }

    public async Task UpdateAsync(int userId, int laborId, UpdateLaborDto dto)
    {
        var labor = GetOwned(userId, laborId) ?? throw new KeyNotFoundException("Labor record not found.");
        
        await EnsureFarmOwnedAsync(userId, dto.FarmId);

        labor.FarmId = dto.FarmId;
        labor.WorkerName = dto.WorkerName;
        labor.WorkDate = dto.WorkDate;
        labor.Task = dto.Task;
        labor.DailyWage = dto.DailyWage;
        labor.PaymentStatus = dto.PaymentStatus;
        labor.Notes = dto.Notes;
        labor.UpdatedAt = DateTime.UtcNow;
        _uow.Labors.Update(labor);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId, int laborId)
    {
        var labor = GetOwned(userId, laborId) ?? throw new KeyNotFoundException("Labor record not found.");
        labor.IsDeleted = true;
        _uow.Labors.Update(labor);
        await _uow.SaveChangesAsync();
    }

    private Domain.Entities.Labor? GetOwned(int userId, int laborId)
    {
        var labor = _uow.Labors.Query(l => l.Farm).FirstOrDefault(l => l.Id == laborId && l.UserId == userId);
        return labor == null || labor.IsDeleted ? null : labor;
    }

    private async Task EnsureFarmOwnedAsync(int userId, int farmId)
    {
        var farm = await _uow.Farms.GetByIdAsync(farmId);
        if (farm == null || farm.UserId != userId || farm.IsDeleted)
            throw new UnauthorizedAccessException("Farm not found or not owned by the current user.");
    }
}
