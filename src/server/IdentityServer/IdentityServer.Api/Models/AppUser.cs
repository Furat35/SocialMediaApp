using BuildingBlocks.Models;

namespace IdentityServer.Api.Models
{
    public class AppUser : BaseEntity
    {
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string ProfileImage { get; set; }
        public string? Bio { get; set; }
        public Guid PasswordSalt { get; set; }
        public string HashedPassword { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
