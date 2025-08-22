using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Behaviours;

namespace Posts.Api.Core.Application.Features.Posts.LikePost
{
    public class LikePostCommand : IRequest<ResponseDto<bool>>, IRequiresFollowCheck
    {
        public int PostId { get; set; }

        public int FollowerId { get; set; }
    }
}
