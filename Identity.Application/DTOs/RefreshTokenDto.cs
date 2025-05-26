using Identity.Core.Entities;

namespace Identity.Application.DTOs
{
    public class RefreshTokenDto
    {
        public string Username { get; set; } = null!;

        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }

        public static RefreshTokenDto Map(RefreshToken refreshToken)
        => new()
        {
            Username = refreshToken.User.UserName!,
            Token = refreshToken.Token,
            ExpiryDate = refreshToken.ExpiryDate,
            IsActive = refreshToken.IsActive,
        };
    }
}