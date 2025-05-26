using Identity.Core.Entities;

namespace Identity.Application.Interfaces
{
    public interface IAuthService
    {
        string GenerateAccessToken(string id,string username, string[] roles);
        RefreshToken GenerateRefreshToken();
    }
}
