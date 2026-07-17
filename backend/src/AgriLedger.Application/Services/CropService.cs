using AgriLedger.Application.DTOs.Crop;
using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Entities;
using AutoMapper;

namespace AgriLedger.Application.Services;

public class CropService : ICropService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CropService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<CropDto>> GetAllForUserAsync(int userId, int? farmId)
    {
        var query = _uow.Crops.Query(c => c.Farm)
            .Where(c => !c.IsDeleted && c.Farm!.UserId == userId);
        if (farmId.HasValue)
            query = query.Where(c => c.FarmId == farmId.Value);

        var crops = query.OrderByDescending(c => c.CreatedAt).ToList();
        return _mapper.Map<List<CropDto>>(crops);
    }

    public async Task<CropDto?> GetByIdAsync(int userId, int cropId)
    {
        var crop = await GetOwnedCropAsync(userId, cropId);
        return crop == null ? null : _mapper.Map<CropDto>(crop);
    }

    public async Task<CropDto> CreateAsync(int userId, CreateCropDto dto)
    {
        await EnsureFarmOwnedAsync(userId, dto.FarmId);

        var crop = _mapper.Map<Crop>(dto);
        await _uow.Crops.AddAsync(crop);
        await _uow.SaveChangesAsync();
        return _mapper.Map<CropDto>(crop);
    }

    public async Task UpdateAsync(int userId, int cropId, UpdateCropDto dto)
    {
        var crop = await GetOwnedCropAsync(userId, cropId)
            ?? throw new KeyNotFoundException("Crop not found.");

        crop.Name = dto.Name;
        crop.Variety = dto.Variety;
        crop.PlantDate = dto.PlantDate;
        crop.ExpectedHarvestDate = dto.ExpectedHarvestDate;
        crop.ActualHarvestDate = dto.ActualHarvestDate;
        crop.Status = dto.Status;
        crop.Notes = dto.Notes;
        crop.UpdatedAt = DateTime.UtcNow;

        _uow.Crops.Update(crop);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId, int cropId)
    {
        var crop = await GetOwnedCropAsync(userId, cropId)
            ?? throw new KeyNotFoundException("Crop not found.");

        crop.IsDeleted = true;
        _uow.Crops.Update(crop);
        await _uow.SaveChangesAsync();
    }

    private async Task<Crop?> GetOwnedCropAsync(int userId, int cropId)
    {
        var crop = _uow.Crops.Query(c => c.Farm).FirstOrDefault(c => c.Id == cropId);
        if (crop == null || crop.IsDeleted) return null;
        await EnsureFarmOwnedAsync(userId, crop.FarmId);
        return crop;
    }

    private async Task EnsureFarmOwnedAsync(int userId, int farmId)
    {
        var farm = await _uow.Farms.GetByIdAsync(farmId);
        if (farm == null || farm.UserId != userId || farm.IsDeleted)
            throw new UnauthorizedAccessException("Farm not found or not owned by the current user.");
    }
}
