using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Identity.Infrastructure.Auth
{
    internal class JwtService(IOptions<JwtAuthenticationSettings> settings) : IAuthService
    {
        public string GenerateAccessToken(string id,string username, string[] roles)
        {
            List<Claim> claims = [
                new(JwtRegisteredClaimNames.Sub, id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Name, username)
            ];

            foreach (var role in roles)
            {
                claims.Add(new(ClaimTypes.Role, role));
            }
            

            RsaSecurityKey rsaSecurityKey = GetRsaKey();
            var signingCred = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

            var securitytoken = new JwtSecurityToken(
                issuer: settings.Value.Issuer,
                audience: settings.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCred
            );

            return new JwtSecurityTokenHandler().WriteToken(securitytoken);
        }


        public RefreshToken GenerateRefreshToken()
        {
            return new()
            {
                Token = GenerateRandomToken(),
                Created = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };
        }


        // helper
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
            string pemKey = File.ReadAllText(settings.Value.PrivateKeyPath);
            rsaKey.ImportFromPem(pemKey);
            var rsaSecurityKey = new RsaSecurityKey(rsaKey);
            return rsaSecurityKey;
        }
    }
}
