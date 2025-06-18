namespace IdentityServer.Api.Business.Dtos
{
    public class RefreshTokenRequestModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
