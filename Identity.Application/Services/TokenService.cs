using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace Identity.Application.Services
{
    public class TokenService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IRefreshTokenRepository refreshTokenRepository)
    {
        public (string, RefreshToken) GenerateTokens(string username)
        {
            var accessToken = GenerateAccessToken(username);
            var refreshToken = GenerateRefreshToken();

            return (accessToken, refreshToken);
        }

        public string GenerateAccessToken(string username)
        {
            List<Claim> claims = [
                new(JwtRegisteredClaimNames.Sub, username),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Name, username)
            ];

            AddUserRolesToClaims(username, claims);

            RsaSecurityKey rsaSecurityKey = GetRsaKey();
            var signingCred = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

            var securitytoken = new JwtSecurityToken(
                issuer: configuration.GetSection("Jwt:Issuer").Value!,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCred
            );

            return new JwtSecurityTokenHandler().WriteToken(securitytoken);
        }

        public static RefreshToken GenerateRefreshToken()
        {
            return new()
            {
                Token = GenerateRandomToken(),
                Created = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };
        }

        public static string GenerateRandomToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private RsaSecurityKey GetRsaKey()
        {
            var rsaKey = RSA.Create();
            string pemKey = File.ReadAllText(configuration.GetSection("Jwt:PrivateKeyPath").Value!);
            rsaKey.ImportFromPem(pemKey);
            var rsaSecurityKey = new RsaSecurityKey(rsaKey);
            return rsaSecurityKey;
        }

        private async void AddUserRolesToClaims(string username, List<Claim> claims)
        {
            var user = await userManager.FindByNameAsync(username) ?? throw new ArgumentException("Invalid User.");
            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new(ClaimTypes.Role, role));
            }
        }

        // Debug
        public async Task<List<RefreshTokenDto>> GetAllRefreshToken()
        {
            var refreshTokens = await refreshTokenRepository.GetAllRefreshTokenAsync();

            return refreshTokens.Select(rt => new RefreshTokenDto()
            {
                Token = rt.Token,
                ExpiryDate = rt.ExpiryDate,
                Username = rt.User.UserName ?? string.Empty,
                IsActive = rt.IsActive,
            }).ToList();
        }
    }
}
