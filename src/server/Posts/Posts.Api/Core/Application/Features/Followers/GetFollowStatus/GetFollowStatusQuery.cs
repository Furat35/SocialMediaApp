using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Followers;
using Posts.Api.Core.Domain.Enums;

namespace Posts.Api.Core.Application.Features.Followers.GetFollowStatus
{
    public class GetFollowStatusQuery : IRequest<ResponseDto<FollowerListDto>>
    {
        public int UserId { get; set; }
    }
}
