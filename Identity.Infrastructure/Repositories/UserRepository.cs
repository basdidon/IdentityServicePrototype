using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories
{
    internal class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        public async Task<ApplicationUser?> GetUserByRefreshTokenAsync(string token)
        {
            return await context.Users
                .Include(user => user.RefreshTokens)
                .SingleOrDefaultAsync(user => user.RefreshTokens.Any(rt => rt.Token == token));
        }
    }
}
