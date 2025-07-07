using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Friends.GetFollowRequests
{
    public class GetFollowRequestsQuery : PaginationRequestModel, IRequest<PaginationResponseModel<GetFollowRequestsResponseQuery>>
    {
    }

    public class GetFollowRequestsResponseQuery
    {
        public int Id { get; set; }
        public int RequestingUserId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
