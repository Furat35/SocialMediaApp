namespace IdentityServer.Api.Business.Dtos
{
    public class LoginResponseModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
    }
}
