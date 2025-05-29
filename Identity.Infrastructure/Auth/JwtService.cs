using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Identity.Infrastructure.Auth
{
    internal class JwtService(IOptions<JwtIssuerOptions> options) : IAuthService
    {
        public string GenerateAccessToken(string id, string username, string[] roles)
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
                issuer: options.Value.Issuer,
                audience: options.Value.Audience,
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
            string pemKey = File.ReadAllText(options.Value.PrivateKeyPath);
            rsaKey.ImportFromPem(pemKey);
            var rsaSecurityKey = new RsaSecurityKey(rsaKey)
            {
                KeyId = options.Value.KeyId
            };
            return rsaSecurityKey;
        }

        public JsonWebKey GetJsonWebKey()
        {
            // Load the private key from the privatekey.pem
            string publicKeyPem = System.IO.File.ReadAllText(options.Value.PublicKeyPath);
            using RSA rsa = RSA.Create();
            // Import the private key from XML
            rsa.ImportFromPem(publicKeyPem);

            // Extract the public key from the private key
            RSAParameters publicKeyParameters = rsa.ExportParameters(false); // 'false' means public key only

            // Convert the public key to JWKS format
            JsonWebKey jwk = new()
            {
                Kty = "RSA",
                Use = "sig",
                Kid = options.Value.KeyId,// Key identifier (set a unique ID for your key)
                E = Base64UrlEncode(publicKeyParameters.Exponent!),
                N = Base64UrlEncode(publicKeyParameters.Modulus!),
                Alg = "RS256"
            };

            return jwk;
        }

        // Helper method to base64url encode without padding
        static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
    }
}
