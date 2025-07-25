using BuildingBlocks.Models;

namespace IdentityServer.Api.Business.Dtos
{
    public class UserRequestDto : PaginationRequestModel
    {
        public string SearchKey { get; set; }
    }
}
