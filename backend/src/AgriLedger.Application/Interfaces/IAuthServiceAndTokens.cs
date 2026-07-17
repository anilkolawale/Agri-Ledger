using AgriLedger.Domain.Entities;

namespace AgriLedger.Application.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IJwtTokenGenerator
{
    (string token, DateTime expiresAt) GenerateToken(User user);
}

public interface ICurrentUserService
{
    int UserId { get; }
    bool IsAuthenticated { get; }
}
