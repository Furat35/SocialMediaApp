using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Behaviours;

namespace Posts.Api.Core.Application.Features.Posts.RemovePostComment
{
    public class RemovePostCommentCommand : IRequest<ResponseDto<bool>>, IRequiresFollowCheck
    {
        public int PostId { get; set; }
        public int CommentId { get; set; }

        public int FollowerId { get; set; }
    }
}
