using AgriLedger.Application.DTOs.Auth;
using AgriLedger.Application.Interfaces;
using AgriLedger.Domain.Entities;

namespace AgriLedger.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public AuthService(IUnitOfWork uow, IPasswordHasher hasher, IJwtTokenGenerator jwt)
    {
        _uow = uow;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var existing = await _uow.Users.FindAsync(u => u.Email == dto.Email);
        if (existing.Any())
            throw new InvalidOperationException("An account with this email already exists.");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = _hasher.Hash(dto.Password),
            PreferredLanguage = dto.PreferredLanguage
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var users = await _uow.Users.FindAsync(u => u.Email == dto.Email);
        var user = users.FirstOrDefault();
        if (user == null || !_hasher.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return BuildAuthResponse(user);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequestDto dto)
    {
        var users = await _uow.Users.FindAsync(u => u.Email == dto.Email);
        var user = users.FirstOrDefault();
        if (user == null) return; // Don't leak whether the email exists.

        user.PasswordResetToken = Guid.NewGuid().ToString("N");
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync();

        // TODO: integrate an email provider (SendGrid / SMTP) to actually send the reset link/token.
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto dto)
    {
        var users = await _uow.Users.FindAsync(u => u.Email == dto.Email);
        var user = users.FirstOrDefault();
        if (user == null || user.PasswordResetToken != dto.Token ||
            user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            throw new InvalidOperationException("Invalid or expired reset token.");

        user.PasswordHash = _hasher.Hash(dto.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto dto)
    {
        var user = await _uow.Users.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        if (!_hasher.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("Current password is incorrect.");

        user.PasswordHash = _hasher.Hash(dto.NewPassword);
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync();
    }

    public async Task UpdateProfileAsync(int userId, UpdateProfileRequestDto dto)
    {
        var user = await _uow.Users.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.PreferredLanguage = dto.PreferredLanguage;
        user.Address = dto.Address;
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync();
    }

    private AuthResponseDto BuildAuthResponse(User user)
    {
        var (token, expiresAt) = _jwt.GenerateToken(user);
        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PreferredLanguage = user.PreferredLanguage,
            Role = user.Role,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address
        };
    }

}
