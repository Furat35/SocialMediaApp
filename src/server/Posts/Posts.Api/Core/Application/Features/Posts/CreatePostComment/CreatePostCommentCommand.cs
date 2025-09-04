using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Behaviours;

namespace Posts.Api.Core.Application.Features.Posts.CreatePostComment
{
    public class CreatePostCommentCommand : IRequest<ResponseDto<bool>>, IRequiresFollowCheck
    {
        //public int UserId { get; set; }
        public int FollowerId { get; set; }
        public int PostId { get; set; }
        public string UserComment { get; set; }

    }
}
