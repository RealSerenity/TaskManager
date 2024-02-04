using System.IdentityModel.Tokens.Jwt;

namespace TaskManager.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(string userId, string userName);
    }
}