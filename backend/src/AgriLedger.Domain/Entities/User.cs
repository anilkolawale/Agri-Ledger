using AgriLedger.Domain.Common;

namespace AgriLedger.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "en"; // en | mr
    public string Role { get; set; } = "Farmer"; // Farmer | Admin | Manager
    public string Address { get; set; } = string.Empty;
    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetTokenExpiry { get; set; }

    public ICollection<Farm> Farms { get; set; } = new List<Farm>();
}
