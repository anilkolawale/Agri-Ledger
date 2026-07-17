using AgriLedger.Application.DTOs.Farm;
using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Entities;
using AutoMapper;

namespace AgriLedger.Application.Services;

public class FarmService : IFarmService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public FarmService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<FarmDto>> GetAllForUserAsync(int userId)
    {
        var farms = _uow.Farms.Query(f => f.Crops)
            .Where(f => f.UserId == userId && !f.IsDeleted)
            .OrderByDescending(f => f.CreatedAt)
            .ToList();
        return _mapper.Map<List<FarmDto>>(farms);
    }

    public async Task<FarmDto?> GetByIdAsync(int userId, int farmId)
    {
        var farm = await GetOwnedFarmAsync(userId, farmId);
        return farm == null ? null : _mapper.Map<FarmDto>(farm);
    }

    public async Task<FarmDto> CreateAsync(int userId, CreateFarmDto dto)
    {
        var farm = _mapper.Map<Farm>(dto);
        farm.UserId = userId;
        await _uow.Farms.AddAsync(farm);
        await _uow.SaveChangesAsync();
        return _mapper.Map<FarmDto>(farm);
    }

    public async Task UpdateAsync(int userId, int farmId, UpdateFarmDto dto)
    {
        var farm = await GetOwnedFarmAsync(userId, farmId)
            ?? throw new KeyNotFoundException("Farm not found.");

        farm.Name = dto.Name;
        farm.AreaInAcres = dto.AreaInAcres;
        farm.Location = dto.Location;
        farm.Notes = dto.Notes;
        farm.UpdatedAt = DateTime.UtcNow;

        _uow.Farms.Update(farm);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId, int farmId)
    {
        var farm = await GetOwnedFarmAsync(userId, farmId)
            ?? throw new KeyNotFoundException("Farm not found.");

        farm.IsDeleted = true; // Soft delete keeps historical expense/income records intact.
        _uow.Farms.Update(farm);
        await _uow.SaveChangesAsync();
    }

    private Task<Farm?> GetOwnedFarmAsync(int userId, int farmId)
    {
        var farm = _uow.Farms.Query(f => f.Crops).FirstOrDefault(f => f.Id == farmId);
        if (farm == null || farm.UserId != userId || farm.IsDeleted) return Task.FromResult<Farm?>(null);
        return Task.FromResult<Farm?>(farm);
    }
}
