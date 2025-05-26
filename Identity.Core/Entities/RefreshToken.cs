using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; } = null!;

        public DateTime Created { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime? Revoked { get; set; } = null;

        public bool IsActive => Revoked == null && ExpiryDate > DateTime.UtcNow;

        public ApplicationUser User { get; set; } = null!;
    }
}
