using Identity.Core.Entities;

namespace Identity.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        public Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);
        public Task SaveRefreshTokenAsync(string userId, RefreshToken refreshToken, CancellationToken ct = default);
        public Task RevokeRefreshTokenAsync(string token, CancellationToken ct = default);
        public Task RevokeAllRefreshTokensAsync(string userId, CancellationToken ct = default);
    }
}
