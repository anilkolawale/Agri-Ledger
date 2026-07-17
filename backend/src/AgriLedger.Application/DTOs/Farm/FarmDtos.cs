namespace AgriLedger.Application.DTOs.Farm;

public class FarmDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double? AreaInAcres { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public int CropCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateFarmDto
{
    public string Name { get; set; } = string.Empty;
    public double? AreaInAcres { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFarmDto : CreateFarmDto { }
