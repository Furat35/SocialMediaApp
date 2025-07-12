using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Followers;
using Posts.Api.Core.Domain.Enums;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowersByUserId
{
    public class GetFollowersByUserIdQuery : PaginationRequestModel, IRequest<PaginationResponseModel<FollowerListDto>>
    {
        public int UserId { get; set; }
        public FollowStatus Status { get; set; } = FollowStatus.Following;
    }
}
