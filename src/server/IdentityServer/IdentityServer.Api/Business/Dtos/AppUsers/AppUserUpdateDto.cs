namespace IdentityServer.Api.Business.Dtos.AppUsers
{
    public class AppUserUpdateDto
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
        public string? Bio { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
