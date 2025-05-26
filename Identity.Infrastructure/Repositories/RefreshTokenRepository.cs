using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories
{
    internal class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
    {
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await context.RefreshTokens
                .Include(t => t.User)
                .SingleOrDefaultAsync(x => x.Token == token);
        }

        public async Task<ApplicationUser?> GetUserByRefreshTokenAsync(string token)
        {
            var refreshToken = await GetRefreshTokenAsync(token);
            return refreshToken?.User;
        }

        public async Task SaveRefreshTokenAsync(string userId, RefreshToken newRefreshToken)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new KeyNotFoundException("user not found.");

            user.RefreshTokens.Add(newRefreshToken);

            await context.SaveChangesAsync();
        }

        // Debug
        public async Task<List<RefreshToken>> GetAllRefreshTokenAsync()
        {
            // return RefreshTokenDtos from here
            return await context.RefreshTokens
                .Include(rt => rt.User)
                .ToListAsync();
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await context.RefreshTokens.FindAsync(token) ?? throw new Exception();

            refreshToken.Revoked = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }
}
