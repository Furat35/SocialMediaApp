using BuildingBlocks.Models;
using MediatR;

namespace Posts.Api.Core.Application.Features.Posts.CreatePostComment
{
    public class CreatePostCommentCommand : IRequest<ResponseDto<bool>>
    {
        public int PostId { get; set; }
        public string UserComment { get; set; }
    }
}
