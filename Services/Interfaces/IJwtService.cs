using System.Security.Claims;

namespace Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string email, string role);
    IEnumerable<Claim> DecodeToken(string token);
    string? GetUserIdFromToken(string token);
}