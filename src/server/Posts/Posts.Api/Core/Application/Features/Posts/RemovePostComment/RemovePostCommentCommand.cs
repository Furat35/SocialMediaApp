using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Posts.RemovePostComment
{
    public class RemovePostCommentCommand : IRequest<ResponseDto<bool>>
    {
        public int PostId { get; set; }
        public int CommentId { get; set; }
    }
}
