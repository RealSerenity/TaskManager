using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Text.Json;
using TaskManager.Data;

namespace TaskManager.Services.Impl
{
    public class JwtServiceImpl : IJwtService
    {
        private readonly string _secretKey;
        private static readonly int LifetimeInHours = 8;
        private static IConfiguration _config;

        public JwtServiceImpl(IConfiguration configuration)
        {
            _config = configuration;
            _secretKey = configuration.GetSection("JwtSettings:Key").Value;
        }

        public string GenerateJwtToken(string userId, string userName)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var token = new JwtSecurityToken(
                issuer: _config.GetSection("JwtSettings:Issuer").Value,
                audience: _config.GetSection("JwtSettings:Audience").Value,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8), // Token'in geçerlilik süresi
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            ) ;

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
