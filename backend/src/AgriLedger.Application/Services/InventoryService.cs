using AgriLedger.Application.DTOs.Inventory;
using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Entities;
using AutoMapper;

namespace AgriLedger.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public InventoryService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<InventoryItemDto>> GetAllForUserAsync(int userId, int? farmId)
    {
        var query = _uow.InventoryItems.Query(i => i.Farm).Where(i => i.UserId == userId && !i.IsDeleted);
        if (farmId.HasValue) query = query.Where(i => i.FarmId == farmId.Value);
        var items = query.OrderBy(i => i.Name).ToList();
        return _mapper.Map<List<InventoryItemDto>>(items);
    }

    public async Task<InventoryItemDto?> GetByIdAsync(int userId, int itemId)
    {
        var item = GetOwned(userId, itemId);
        return item == null ? null : _mapper.Map<InventoryItemDto>(item);
    }

    public async Task<InventoryItemDto> CreateAsync(int userId, CreateInventoryItemDto dto)
    {
        if (dto.FarmId.HasValue)
        {
            await EnsureFarmOwnedAsync(userId, dto.FarmId.Value);
        }

        var item = _mapper.Map<InventoryItem>(dto);
        item.UserId = userId;
        await _uow.InventoryItems.AddAsync(item);
        await _uow.SaveChangesAsync();
        return _mapper.Map<InventoryItemDto>(item);
    }

    public async Task UpdateAsync(int userId, int itemId, UpdateInventoryItemDto dto)
    {
        var item = GetOwned(userId, itemId) ?? throw new KeyNotFoundException("Inventory item not found.");
        
        if (dto.FarmId.HasValue)
        {
            await EnsureFarmOwnedAsync(userId, dto.FarmId.Value);
        }

        item.Name = dto.Name;
        item.Type = dto.Type;
        item.QuantityAvailable = dto.QuantityAvailable;
        item.Unit = dto.Unit;
        item.ReorderThreshold = dto.ReorderThreshold;
        item.FarmId = dto.FarmId;
        item.Notes = dto.Notes;
        item.UpdatedAt = DateTime.UtcNow;
        _uow.InventoryItems.Update(item);
        await _uow.SaveChangesAsync();
    }

    public async Task AdjustQuantityAsync(int userId, int itemId, AdjustInventoryDto dto)
    {
        var item = GetOwned(userId, itemId) ?? throw new KeyNotFoundException("Inventory item not found.");
        var newQuantity = item.QuantityAvailable + dto.QuantityDelta;
        if (newQuantity < 0)
            throw new InvalidOperationException("Resulting quantity cannot be negative.");
        item.QuantityAvailable = newQuantity;
        item.UpdatedAt = DateTime.UtcNow;
        _uow.InventoryItems.Update(item);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId, int itemId)
    {
        var item = GetOwned(userId, itemId) ?? throw new KeyNotFoundException("Inventory item not found.");
        item.IsDeleted = true;
        _uow.InventoryItems.Update(item);
        await _uow.SaveChangesAsync();
    }

    private InventoryItem? GetOwned(int userId, int itemId)
    {
        var item = _uow.InventoryItems.Query(i => i.Farm).FirstOrDefault(i => i.Id == itemId && i.UserId == userId);
        return item == null || item.IsDeleted ? null : item;
    }

    private async Task EnsureFarmOwnedAsync(int userId, int farmId)
    {
        var farm = await _uow.Farms.GetByIdAsync(farmId);
        if (farm == null || farm.UserId != userId || farm.IsDeleted)
            throw new UnauthorizedAccessException("Farm not found or not owned by the current user.");
    }
}

