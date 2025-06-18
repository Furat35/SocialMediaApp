namespace IdentityServer.Api.Business.Dtos
{
    public class RegisterRequestModel
    {
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
