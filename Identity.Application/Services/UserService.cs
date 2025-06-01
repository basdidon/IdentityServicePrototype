using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Services
{
    public interface IUserService
    {
        Task<(string accessToken, RefreshToken refreshToken)?> LoginAsync(string username, string password);
        Task LogoutAsync(string userId);
        Task<(string accessToken, RefreshToken refreshToken)?> RefreshTokenAsync(string refreshToken);
    }

    public class UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAuthService authService, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepo) : IUserService
    {
        public async Task<(string accessToken, RefreshToken refreshToken)?> LoginAsync(string username, string password)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null || !(await signInManager.CheckPasswordSignInAsync(user, password, false)).Succeeded)
                return null;

            var roles = await userManager.GetRolesAsync(user);
            var accessToken = authService.GenerateAccessToken(user.Id, username, [.. roles]);
            var refreshToken = authService.GenerateRefreshToken();

            await refreshTokenRepo.SaveRefreshTokenAsync(user.Id, refreshToken);
            return (accessToken, refreshToken);
        }

        public async Task LogoutAsync(string userId)
        {
            await refreshTokenRepo.RevokeAllRefreshTokensAsync(userId);
        }

        public async Task<(string accessToken, RefreshToken refreshToken)?> RefreshTokenAsync(string oldToken)
        {
            var refreshToken = await refreshTokenRepo.GetRefreshTokenAsync(oldToken);
            if (refreshToken == null || !refreshToken.IsActive)
                return null;

            var user = await userRepository.GetUserByRefreshTokenAsync(oldToken);
            if (user == null)
                return null;

            await refreshTokenRepo.RevokeRefreshTokenAsync(oldToken);

            var newRefreshToken = authService.GenerateRefreshToken();
            await refreshTokenRepo.SaveRefreshTokenAsync(user.Id, newRefreshToken);
            var roles = await userManager.GetRolesAsync(user);
            var newAccessToken = authService.GenerateAccessToken(user.Id, user.UserName ?? string.Empty, [.. roles]);

            return (newAccessToken, newRefreshToken);
        }
    }
}
