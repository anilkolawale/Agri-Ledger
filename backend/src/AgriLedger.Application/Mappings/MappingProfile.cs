using AgriLedger.Application.DTOs.Crop;
using AgriLedger.Application.DTOs.Expense;
using AgriLedger.Application.DTOs.Farm;
using AgriLedger.Application.DTOs.Income;
using AgriLedger.Application.DTOs.Inventory;
using AgriLedger.Application.DTOs.Labor;
using AgriLedger.Domain.Entities;
using AutoMapper;

namespace AgriLedger.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Farm, FarmDto>()
            .ForMember(d => d.CropCount, o => o.MapFrom(s => s.Crops.Count(c => !c.IsDeleted)))
            .MaxDepth(3);
        CreateMap<CreateFarmDto, Farm>();
        CreateMap<UpdateFarmDto, Farm>();

        CreateMap<Crop, CropDto>()
            .ForMember(d => d.FarmName, o => o.MapFrom(s => s.Farm != null ? s.Farm.Name : string.Empty))
            .MaxDepth(3);
        CreateMap<CreateCropDto, Crop>();
        CreateMap<UpdateCropDto, Crop>();

        CreateMap<Expense, ExpenseDto>()
            .ForMember(d => d.FarmName, o => o.MapFrom(s => s.Farm != null ? s.Farm.Name : string.Empty))
            .ForMember(d => d.CropName, o => o.MapFrom(s => s.Crop != null ? s.Crop.Name : null))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.ExpenseCategory != null ? s.ExpenseCategory.Name : string.Empty))
            .ForMember(d => d.ExpenseTime, o => o.MapFrom(s => s.ExpenseTime.HasValue ? s.ExpenseTime.Value.ToString(@"hh\:mm") : null))
            .MaxDepth(3);
        CreateMap<CreateExpenseDto, Expense>()
            .ForMember(d => d.ExpenseTime, o => o.MapFrom(s => ParseTime(s.ExpenseTime)));
        CreateMap<UpdateExpenseDto, Expense>()
            .ForMember(d => d.ExpenseTime, o => o.MapFrom(s => ParseTime(s.ExpenseTime)));
        CreateMap<ExpenseCategory, ExpenseCategoryDto>();

        CreateMap<Income, IncomeDto>()
            .ForMember(d => d.FarmName, o => o.MapFrom(s => s.Farm != null ? s.Farm.Name : string.Empty))
            .ForMember(d => d.CropName, o => o.MapFrom(s => s.Crop != null ? s.Crop.Name : null))
            .MaxDepth(3);
        CreateMap<CreateIncomeDto, Income>();
        CreateMap<UpdateIncomeDto, Income>();

        CreateMap<Domain.Entities.Labor, LaborDto>()
            .ForMember(d => d.FarmName, o => o.MapFrom(s => s.Farm != null ? s.Farm.Name : string.Empty))
            .MaxDepth(3);
        CreateMap<CreateLaborDto, Domain.Entities.Labor>();
        CreateMap<UpdateLaborDto, Domain.Entities.Labor>();

        CreateMap<InventoryItem, InventoryItemDto>()
            .ForMember(d => d.FarmName, o => o.MapFrom(s => s.Farm != null ? s.Farm.Name : null))
            .ForMember(d => d.IsLowStock, o => o.MapFrom(s => s.QuantityAvailable <= s.ReorderThreshold))
            .MaxDepth(3);
        CreateMap<CreateInventoryItemDto, InventoryItem>();
        CreateMap<UpdateInventoryItemDto, InventoryItem>();
    }

    // Accepts "HH:mm" (from an HTML <input type="time">) or "HH:mm:ss"; returns null for
    // empty/invalid input rather than throwing, since ExpenseTime is optional.
    private static TimeSpan? ParseTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return TimeSpan.TryParse(value, out var result) ? result : null;
    }
}
