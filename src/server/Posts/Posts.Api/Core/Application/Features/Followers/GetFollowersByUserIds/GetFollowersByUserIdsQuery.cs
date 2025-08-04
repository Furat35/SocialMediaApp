using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Followers;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowersByUserIds
{
    public class GetFollowersByUserIdsQuery : PaginationRequestModel, IRequest<PaginationResponseModel<FollowerListDto>>
    {
        public List<int> UserIds { get; set; }
    }
}
