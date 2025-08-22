using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Behaviours;

namespace Posts.Api.Core.Application.Features.Posts.UnlikePost
{
    public class UnlikePostCommand : IRequest<ResponseDto<bool>>, IRequiresFollowCheck
    {
        public int PostId { get; set; }

        public int FollowerId { get; set; }
    }
}
