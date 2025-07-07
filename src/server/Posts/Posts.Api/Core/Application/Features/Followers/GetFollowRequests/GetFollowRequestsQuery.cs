using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Followers;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowRequests
{
    public class GetFollowRequestsQuery : PaginationRequestModel, IRequest<PaginationResponseModel<FollowerListDto>>
    {
    }
}
