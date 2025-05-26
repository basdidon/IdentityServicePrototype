using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
