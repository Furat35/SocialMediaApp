using BuildingBlocks.Models;

namespace IdentityServer.Api.Business.Dtos
{
    public class UserRequestDto : PaginationRequestModel
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
    }
}
