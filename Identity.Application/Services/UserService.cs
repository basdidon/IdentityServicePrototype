using Identity.Application.Interfaces;
using Identity.Core.Entities;

namespace Identity.Application.Services
{
    public interface IUserService
    {
        public Task<RefreshToken?> GetRefreshTokenAsync(string token);
        public Task SaveRefreshTokenAsync(ApplicationUser user, RefreshToken token);
        public Task RevokeRefreshTokenAsync(string token);
        public Task<ApplicationUser?> GetUserByRefreshTokenAsync(string token);
    }


    public class UserService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository) : IUserService
    {
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await refreshTokenRepository.GetRefreshTokenAsync(token);
        }

        public async Task SaveRefreshTokenAsync(ApplicationUser user, RefreshToken token)
        {
            await refreshTokenRepository.SaveRefreshTokenAsync(user.Id, token);
        }

        public async Task<ApplicationUser?> GetUserByRefreshTokenAsync(string token)
        {
            return await userRepository.GetUserByRefreshTokenAsync(token);
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            await refreshTokenRepository.RevokeRefreshTokenAsync(token);
        }
    }
}
