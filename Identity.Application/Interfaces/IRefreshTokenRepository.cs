using Identity.Core.Entities;

namespace Identity.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        public Task<List<RefreshToken>> GetAllRefreshTokenAsync();
        public Task<RefreshToken?> GetRefreshTokenAsync(string token);
        public Task SaveRefreshTokenAsync(string userId, RefreshToken refreshToken);
        public Task RevokeRefreshTokenAsync(string token);
    }
}
