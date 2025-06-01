using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories
{
    internal class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
    {
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default)
        {
            return await context.RefreshTokens
                .Include(t => t.User)
                .SingleOrDefaultAsync(x => x.Token == token,ct);
        }

        public async Task SaveRefreshTokenAsync(string userId, RefreshToken newRefreshToken, CancellationToken ct = default)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: ct) ?? throw new KeyNotFoundException("user not found.");

            user.RefreshTokens.Add(newRefreshToken);

            await context.SaveChangesAsync(ct);
        }

        public async Task RevokeRefreshTokenAsync(string token, CancellationToken ct = default)
        {
            var refreshToken = await context.RefreshTokens.FindAsync([token], ct) ?? throw new Exception();

            refreshToken.Revoked = DateTime.UtcNow;
            await context.SaveChangesAsync( ct);
        }

        public async Task RevokeAllRefreshTokensAsync(string userId, CancellationToken ct = default)
        {
            var refreshToken = await context.RefreshTokens
                .AsNoTracking()
                .AsQueryable()
                .Where(x=>x.User.Id == userId && x.IsActive == true)
                .ToListAsync(ct);
        }
    }
}
