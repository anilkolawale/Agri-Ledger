using AgriLedger.Domain.Enums;

namespace AgriLedger.Application.DTOs.Crop;

public class CropDto
{
    public int Id { get; set; }
    public int FarmId { get; set; }
    public string FarmName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Variety { get; set; }
    public DateTime? PlantDate { get; set; }
    public DateTime? ExpectedHarvestDate { get; set; }
    public DateTime? ActualHarvestDate { get; set; }
    public CropStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateCropDto
{
    public int FarmId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Variety { get; set; }
    public DateTime? PlantDate { get; set; }
    public DateTime? ExpectedHarvestDate { get; set; }
    public CropStatus Status { get; set; } = CropStatus.Planned;
    public string? Notes { get; set; }
}

public class UpdateCropDto : CreateCropDto
{
    public DateTime? ActualHarvestDate { get; set; }
}
