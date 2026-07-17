using AgriLedger.Domain.Common;

namespace AgriLedger.Domain.Entities;

public class MandiPrice : BaseEntity
{
    public string MarketName { get; set; } = string.Empty; // e.g. Nashik APMC, Pune APMC, Mumbai APMC, Nagpur APMC
    public string CropName { get; set; } = string.Empty;   // e.g. Onion, Tomato, Mango, Wheat, Soybean
    public decimal MinPrice { get; set; }                  // per Quintal (100 kg)
    public decimal MaxPrice { get; set; }
    public decimal ModalPrice { get; set; }                // Typical selling price
    public string Unit { get; set; } = "Quintal";
    public DateTime PriceDate { get; set; }
}
