using Identity.Core.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Application.Interfaces
{
    public interface IAuthService
    {
        string GenerateAccessToken(string id,string username, string[] roles);
        RefreshToken GenerateRefreshToken();
        JsonWebKey GetJsonWebKey();
    }
}
