using Identity.Core.Entities;

namespace Identity.Application.Interfaces
{
    public interface IUserRepository
    {
        public Task<ApplicationUser?> GetUserByRefreshTokenAsync(string token);
    }
}
